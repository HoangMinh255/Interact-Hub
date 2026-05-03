import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { commentsAPI, notificationsAPI } from "../api";
import type { Post } from "../types";
import { useAuth } from "../context/AuthContext";

interface CommentItem {
  id: string;
  content: string;
  authorName?: string;
  authorAvatar?: string;
  createdAt?: string;
  parentCommentId?: string | null;
}

interface PostCardProps {
  post: Post;
  onDelete?: () => void;
}

const getVisibilityLabel = (visibility: number) => {
  switch (visibility) {
    case 1:
      return "Bạn bè";
    case 2:
      return "Chỉ mình tôi";
    default:
      return "Công khai";
  }
};

function PostCard({ post, onDelete }: PostCardProps) {
  const { user } = useAuth(); 
  
  const [liked, setLiked] = useState(false);
  const [likes, setLikes] = useState(0);
  const [showComment, setShowComment] = useState(false);
  
  const [comment, setComment] = useState("");
  const [replyText, setReplyText] = useState("");
  const [replyingTo, setReplyingTo] = useState<string | null>(null);

  const [comments, setComments] = useState<CommentItem[]>([]);
  const [localComments, setLocalComments] = useState<CommentItem[]>([]);

  useEffect(() => {
    if (!showComment) return;

    const loadComments = async () => {
      try {
        const response = await commentsAPI.getByPostId(post.id);
        const fetched = Array.isArray(response.data) ? response.data : [];

        const normalizedComments = fetched.map((c: any) => ({
          id: c.id || c.Id,
          content: c.content || c.Content,
          authorName: c.authorName || c.AuthorName || "Người dùng",
          authorAvatar: c.authorAvatar || c.AuthorAvatar,
          createdAt: c.createdAt || c.CreatedAt,
          parentCommentId: c.parentCommentId || c.ParentCommentId || null
        }));

        setComments(normalizedComments);
      } catch {
        setComments([]);
      }
    };

    void loadComments();
  }, [post.id, showComment]);

  const handleLike = () => {
    setLiked(!liked);
    setLikes(liked ? likes - 1 : likes + 1);
  };

  const submitComment = async (parentId: string | null = null) => {
    const textToSubmit = parentId ? replyText : comment;
    if (!textToSubmit.trim()) return;

    try {
      await commentsAPI.create(post.id, textToSubmit.trim(), parentId);

      const targetId = post.author?.id ?? (post as any).authorId ?? null;
      if (targetId && user?.id && targetId !== user.id) {
        await notificationsAPI.create({
          recipientId: targetId,
          actorId: user.id,
          type: 1, 
          content: `${user.fullName || "Một người"} đã ${parentId ? "phản hồi bình luận" : "bình luận"} trong bài viết của bạn.`,
          relatedEntityType: "Post",
          relatedEntityId: post.id,
          createdAt: new Date().toISOString()
        });
      }

      const newComment: CommentItem = {
        id: Date.now().toString(),
        content: textToSubmit.trim(),
        authorName: user?.fullName || "Bạn",
        authorAvatar: user?.avatarUrl,
        parentCommentId: parentId
      };

      setLocalComments((prev) => [...prev, newComment]);
      
      if (parentId) {
        setReplyText("");
        setReplyingTo(null); 
      } else {
        setComment("");
      }
    } catch (error) {
      console.error("Lỗi khi bình luận:", error);
    }
  };

  const allComments = [...comments, ...localComments];
  const topLevelComments = allComments.filter(c => !c.parentCommentId);

  // Tính tổng số bình luận chuẩn nhất để không bị chớp "0"
  const totalCommentsCount = Math.max((post.commentCount || 0) + localComments.length, allComments.length);

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4">
      {/* --- PHẦN ĐẦU BÀI VIẾT --- */}
      <div className="flex items-center gap-3 mb-3">
        {(() => {
          const targetId = post.author?.id ?? (post as any).authorId ?? null;
          const avatar = post.authorAvatar ?? "";
          return (
            <Link to={targetId ? `/profile/${targetId}` : "#"} className="shrink-0">
              {avatar ? (
                <img src={avatar} className="w-9 h-9 rounded-full object-cover border border-gray-200" referrerPolicy="no-referrer" />
              ) : (
                <div className="w-9 h-9 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">{(post.authorName || "U").charAt(0).toUpperCase()}</div>
              )}
            </Link>
          );
        })()}
        <div className="flex-1">
          {(() => {
            const targetId = post.author?.id ?? (post as any).authorId ?? null;
            const name = post.authorName || "Unknown";
            return targetId 
              ? <Link to={`/profile/${targetId}`} className="text-sm font-medium text-gray-800 hover:underline">{name}</Link>
              : <p className="text-sm font-medium text-gray-800">{name}</p>;
          })()}
          <div className="flex items-center gap-2 text-xs text-gray-400">
            <span>{post.createdAt}</span>
            <span className="inline-flex items-center rounded-full bg-gray-100 px-2 py-0.5 text-[11px] font-medium text-gray-500">
              {getVisibilityLabel(post.visibility)}
            </span>
          </div>
        </div>
        {onDelete && (
          <button onClick={onDelete} className="text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50">
            🗑️ Xóa
          </button>
        )}
      </div>

      <p className="text-sm text-gray-700 mb-3">{post.content}</p>

      {post.mediaUrls && post.mediaUrls.length > 0 && (
        <div className="flex flex-col gap-2 mb-3">
          {post.mediaUrls.map((url, idx) => (
            <img key={idx} src={url} alt="media" className="block w-full h-auto rounded-lg" />
          ))}
        </div>
      )}

      {/* --- CÁC NÚT TƯƠNG TÁC --- */}
      <div className="flex border-t border-gray-100 pt-2 gap-1">
        <button
          onClick={handleLike}
          className={`flex-1 flex items-center justify-center gap-1 py-1.5 text-sm rounded-lg ${liked ? "text-red-500 bg-red-50" : "text-gray-500 hover:bg-gray-50"}`}
        >
          ❤️ {likes} Thích
        </button>
        <button
          onClick={() => setShowComment(!showComment)}
          className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm text-gray-500 hover:bg-gray-50 rounded-lg"
        >
          {/* CẬP NHẬT: Dùng biến totalCommentsCount ở đây */}
          💬 {totalCommentsCount} Bình luận
        </button>
        <button
          onClick={() => alert("Đã chia sẻ bài viết!")}
          className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm text-gray-500 hover:bg-gray-50 rounded-lg"
        >
          🔁 Chia sẻ
        </button>
      </div>

      {/* --- KHU VỰC BÌNH LUẬN --- */}
      {showComment && (
        <div className="mt-3 border-t border-gray-100 pt-3">
          
          {topLevelComments.map((c) => {
            const replies = allComments.filter(reply => reply.parentCommentId === c.id);
            const hasReplies = replies.length > 0;
            const isReplying = replyingTo === c.id;

            return (
              <div key={c.id} className="mb-4">
                
                {/* 1. BÌNH LUẬN CHA GỐC */}
                <div className="flex gap-2">
                  {c.authorAvatar ? (
                    <img src={c.authorAvatar} className="w-8 h-8 rounded-full object-cover shrink-0 border border-gray-100 z-10" />
                  ) : (
                    <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs shrink-0 font-medium z-10">
                      {(c.authorName ?? "U").charAt(0).toUpperCase()}
                    </div>
                  )}
                  
                  <div className="flex flex-col">
                    <div className="bg-gray-100 rounded-2xl px-3 py-2 text-sm text-gray-800">
                      <span className="font-semibold block text-[13px]">{c.authorName}</span>
                      <p>{c.content}</p>
                    </div>
                    <button 
                      onClick={() => setReplyingTo(isReplying ? null : c.id)}
                      className="text-[12px] text-gray-500 font-medium ml-2 mt-1 hover:underline text-left w-fit"
                    >
                      Phản hồi
                    </button>
                  </div>
                </div>

                {/* 2. CÁC BÌNH LUẬN CON */}
                {(hasReplies || isReplying) && (
                  <div className="ml-4 pl-4 border-l-2 border-gray-200 mt-1 flex flex-col gap-3 pt-2">
                    
                    {replies.map((reply) => (
                      <div key={reply.id} className="flex gap-2 relative">
                        <div className="absolute -left-4 top-[14px] w-4 h-[2px] bg-gray-200"></div>
                        
                        {reply.authorAvatar ? (
                          <img src={reply.authorAvatar} className="w-7 h-7 rounded-full object-cover shrink-0 border border-gray-100 relative z-10" />
                        ) : (
                          <div className="w-7 h-7 rounded-full bg-gray-400 flex items-center justify-center text-white text-[10px] shrink-0 font-medium relative z-10">
                            {(reply.authorName ?? "U").charAt(0).toUpperCase()}
                          </div>
                        )}

                        {/* CẬP NHẬT: Thêm "phản hồi của..." và nút "Phản hồi" cho bình luận con */}
                        <div className="flex flex-col relative z-10">
                          <div className="bg-gray-50 border border-gray-100 rounded-2xl px-3 py-1.5 text-sm text-gray-800">
                            <span className="font-semibold block text-[12px]">
                              {reply.authorName}{" "}
                              <span className="font-normal text-gray-500 text-[11px]">
                                (phản hồi của {c.authorName})
                              </span>
                            </span>
                            <p className="text-[13px]">{reply.content}</p>
                          </div>
                          
                          <button 
                            onClick={() => setReplyingTo(isReplying ? null : c.id)}
                            className="text-[11px] text-gray-500 font-medium ml-2 mt-1 hover:underline text-left w-fit"
                          >
                            Phản hồi
                          </button>
                        </div>
                      </div>
                    ))}

                    {/* 3. Ô NHẬP PHẢN HỒI (Mở chung dưới cùng của nhánh cây) */}
                    {isReplying && (
                      <div className="flex gap-2 relative items-center">
                        <div className="absolute -left-4 top-1/2 w-4 h-[2px] bg-gray-200"></div>
                        
                        {user?.avatarUrl ? (
                          <img src={user.avatarUrl} className="w-7 h-7 rounded-full object-cover shrink-0 border border-gray-100 relative z-10" />
                        ) : (
                          <div className="w-7 h-7 rounded-full bg-blue-500 flex items-center justify-center text-white text-[10px] shrink-0 font-medium relative z-10">
                            {(user?.fullName ?? "U").charAt(0).toUpperCase()}
                          </div>
                        )}

                        <input
                          type="text"
                          autoFocus
                          value={replyText}
                          onChange={(e) => setReplyText(e.target.value)}
                          onKeyDown={(e) => e.key === "Enter" && submitComment(c.id)}
                          placeholder={`Phản hồi ${c.authorName}...`}
                          className="flex-1 bg-gray-100 border border-gray-200 rounded-full px-3 text-[13px] outline-none h-8 relative z-10 focus:border-blue-400"
                        />
                        <button onClick={() => submitComment(c.id)} className="text-blue-500 text-sm font-medium z-10">Gửi</button>
                      </div>
                    )}
                  </div>
                )}

              </div>
            );
          })}

          {/* --- Ô NHẬP BÌNH LUẬN GỐC TỔNG --- */}
          <div className="flex gap-2 mt-4 pt-3 border-t border-gray-50 items-center">
            {user?.avatarUrl ? (
              <img src={user.avatarUrl} className="w-8 h-8 rounded-full object-cover shrink-0 border border-gray-100" />
            ) : (
              <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs shrink-0 font-medium">
                {(user?.fullName ?? "U").charAt(0).toUpperCase()}
              </div>
            )}
            
            <input
              type="text"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && submitComment()} 
              placeholder="Viết bình luận..."
              className="flex-1 bg-gray-100 border border-gray-200 rounded-full px-4 text-sm outline-none h-9 focus:border-blue-400"
            />
            <button onClick={() => submitComment()} className="text-blue-500 text-sm font-medium px-2">Gửi</button>
          </div>
        </div>
      )}
    </div>
  );
}

export default PostCard;