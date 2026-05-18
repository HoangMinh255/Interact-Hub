/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { commentsAPI, notificationsAPI, postsAPI, reportsAPI, resolveMediaUrl } from "../api";
import type { Post } from "../types";
import { useAuth } from "../context/AuthContext";
import ReportModal from "./ReportModal";
import ShareModal from "./ShareModal";
import Avatar from "./ui/avatar";
import { CiHeart, CiShare2 } from "react-icons/ci";
import { FaRegComment} from "react-icons/fa";
import { MdDeleteOutline } from "react-icons/md";



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
  onHashtagClick?: (tag: string) => void;
}

function PostCard({ post, onDelete, onHashtagClick }: PostCardProps) {
  const { user } = useAuth(); 
  
  const [liked, setLiked] = useState(false);
  const [likes, setLikes] = useState(0);
  const [showComment, setShowComment] = useState(false);
  
  const [comment, setComment] = useState("");
  const [replyText, setReplyText] = useState("");
  const [replyingTo, setReplyingTo] = useState<string | null>(null);

  const [comments, setComments] = useState<CommentItem[]>([]);
  const [localComments, setLocalComments] = useState<CommentItem[]>([]);
  
  const [showReportModal, setShowReportModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [showShareModal, setShowShareModal] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editContent, setEditContent] = useState(post.content ?? "");
  const [savingEdit, setSavingEdit] = useState(false);
  const [localContent, setLocalContent] = useState(post.content ?? "");

  useEffect(() => {
    setLocalContent(post.content ?? "");
    setEditContent(post.content ?? "");
  }, [post.content]);

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

  useEffect(() => {
    let mounted = true;
    const loadLikeState = async () => {
      try {
        const [likedRes, countRes] = await Promise.all([
          postsAPI.isLiked(post.id),
          postsAPI.getLikeCount(post.id),
        ]);

        if (!mounted) return;
        setLiked(Boolean(likedRes.data?.data?.liked ?? likedRes.data?.liked ?? likedRes.data));
        const likeCount = likedRes.data?.data?.likeCount ?? countRes.data?.data?.likeCount ?? countRes.data;
        setLikes(Number(likeCount ?? 0));
      } catch {
      }
    };
    void loadLikeState();
    return () => { mounted = false; };
  }, [post.id]);

  const handleLike = async () => {
    try {
      if (liked) {
        await postsAPI.unlike(post.id);
        setLiked(false);
        setLikes((prev) => Math.max(0, prev - 1));
      } else {
        await postsAPI.like(post.id);
        setLiked(true);
        setLikes((prev) => prev + 1);
      }
    } catch (e) {
      console.error("Like action failed", e);
    }
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

  // Tính tổng số bình luận chuẩn nhất
  const totalCommentsCount = Math.max((post.commentCount || 0) + localComments.length, allComments.length);

  // --- KẾT HỢP CÁC HÀM XỬ LÝ MỚI TỪ NHÁNH MOBILE FRIENDLY ---
  const handleReport = async (reason: string) => {
    await reportsAPI.create(post.id, reason);
  };

  const handleShare = async (comment?: string) => {
    await postsAPI.share(post.id, comment);
  };

  const handleConfirmDelete = () => {
    setShowDeleteConfirm(false);
    if (onDelete) {
      onDelete();
    }
  };

  // Hàm tìm tất cả các chữ bắt đầu bằng # và biến chúng thành thẻ <span> màu xanh
  const renderContentWithHashtags = (text?: string) => {
    if (!text) return null;
    
    // Tách text bằng khoảng trắng hoặc xuống dòng, bắt các cụm có dấu #
    const words = text.split(/(\s+)/);
    
    return words.map((word, index) => {
      if (word.startsWith("#") && word.length > 1) {
        return (
          <span key={index} className="text-blue-500 hover:underline cursor-pointer font-medium" 
                onClick={(e) => {e.stopPropagation();
                                  if (onHashtagClick) {
                                    onHashtagClick(word.replace('#', ''));
                                  }
                                }}>
            {word}
          </span>
        );
      }
      return word;
    });
  };

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4">
      {/* --- PHẦN ĐẦU BÀI VIẾT --- */}
      <div className="flex items-center gap-3 mb-3">
        {(() => {
          // Logic render Avatar cho bài Shared
          const rawAvatar = post.isShared ? (post.sharedByAvatar ?? null) : (post.authorAvatar ?? null);
          let displayName = post.isShared ? (post.sharedByName ?? "Unknown") : (post.authorName || "Unknown");
          let targetId = post.isShared 
            ? (post.sharedById ?? null) 
            : (post.author?.id ?? (post as any).authorId ?? null);

          if (targetId) {
            return (
              <Link to={`/profile/${targetId}`} className="shrink-0">
                <Avatar name={displayName} avatarUrl={rawAvatar} size="md" />
              </Link>
            );
          }

          return <Avatar name={displayName} avatarUrl={rawAvatar} size="md" />;
        })()}
        
        <div className="flex-1">
          {(() => {
            // Logic render Tên cho bài Shared
            let targetId = post.isShared 
              ? (post.sharedById ?? null) 
              : (post.author?.id ?? (post as any).authorId ?? null);
            let name = post.isShared ? (post.sharedByName ?? "Unknown") : (post.authorName || "Unknown");
            
            if (targetId) {
              return (
                <div>
                  <Link to={`/profile/${targetId}`} className="text-sm font-medium text-gray-800 hover:underline">{name}</Link>
                  {post.isShared && post.authorName && (
                    <div className="text-xs text-gray-500 mt-0.5">Original by <span className="text-gray-700 font-medium">{post.authorName}</span></div>
                  )}
                </div>
              );
            }
            return (
              <div>
                <p className="text-sm font-medium text-gray-800">{name}</p>
                {post.isShared && post.authorName && (
                  <div className="text-xs text-gray-500 mt-0.5">Original by <span className="text-gray-700 font-medium">{post.authorName}</span></div>
                )}
              </div>
            );
          })()}
          <div className="flex items-center gap-2 text-xs text-gray-400">
            <span>{post.createdAt}</span>
          </div>
        </div>
        
        {(() => {
          const originalAuthorId = post.author?.id ?? (post as any).authorId ?? null;
          const isOriginalAuthor = user?.id === originalAuthorId;

          return (
            <>
              {isOriginalAuthor && (
                <button onClick={() => { setIsEditing(true); setEditContent(localContent); }} className="text-xs text-gray-600 px-2 py-1 rounded hover:bg-gray-50 mr-2">
                  Sửa
                </button>
              )}

              {onDelete && (
                <button onClick={() => setShowDeleteConfirm(true)} className="text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50 inline-flex items-center gap-1">
                  <MdDeleteOutline />
                  <span>Xóa</span>
                </button>
              )}
              {!onDelete && (
                <button
                  onClick={() => setShowReportModal(true)}
                  className="text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50"
                  title="Báo cáo bài viết"
                >
                  🚩 Báo cáo
                </button>
              )}
            </>
          );
        })()}
      </div>

      {post.isShared && post.shareComment && (
        <div className="mb-3 pb-3 border-b border-gray-200">
          <p className="text-sm text-gray-600 italic bg-blue-50 rounded-lg px-3 py-2 border-l-2 border-blue-400">
            "{post.shareComment}"
          </p>
        </div>
      )}

      {isEditing ? (
        <div className="mb-3">
          <textarea
            value={editContent}
            onChange={(e) => setEditContent(e.target.value)}
            className="w-full border border-gray-200 rounded-lg p-3 text-sm h-28 resize-none"
          />
          <div className="flex gap-2 justify-end mt-2">
            <button onClick={() => { setIsEditing(false); setEditContent(localContent); }} className="px-3 py-1 rounded bg-gray-100 hover:bg-gray-200">Hủy</button>
            <button
              onClick={async () => {
                try {
                  setSavingEdit(true);
                  await postsAPI.update(post.id, editContent);
                  setLocalContent(editContent);
                  setIsEditing(false);
                } catch (err) {
                  console.error("Failed to update post:", err);
                } finally {
                  setSavingEdit(false);
                }
              }}
              disabled={savingEdit || !editContent.trim()}
              className="px-3 py-1 rounded bg-blue-500 text-white hover:bg-blue-600 disabled:opacity-50"
            >
              {savingEdit ? "Đang lưu..." : "Lưu"}
            </button>
          </div>
        </div>
      ) : (
        <p className="text-sm text-gray-700 mb-3 whitespace-pre-wrap">
          {renderContentWithHashtags((localContent ?? post.content) + (post.hashtag?.map(tag => ` #${tag}`).join("") ?? ""))}
        </p>
      )}

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
          <CiHeart /> {likes} Thích
        </button>
        <button
          onClick={() => setShowComment(!showComment)}
          className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm text-gray-500 hover:bg-gray-50 rounded-lg"
        >
          <FaRegComment /> {totalCommentsCount} Bình luận
        </button>
        <button
          onClick={() => setShowShareModal(true)}
          className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm text-gray-500 hover:bg-gray-50 rounded-lg"
        >
          <CiShare2 /> Chia sẻ
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
                    <Avatar name={c.authorName ?? "Người dùng"} avatarUrl={c.authorAvatar ?? null} size="sm" />
                  
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
                        
                        <Avatar name={reply.authorName ?? "Người dùng"} avatarUrl={reply.authorAvatar ?? null} size="sm" />

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

                    {/* 3. Ô NHẬP PHẢN HỒI */}
                    {isReplying && (
                      <div className="flex gap-2 relative items-center">
                        <div className="absolute -left-4 top-1/2 w-4 h-[2px] bg-gray-200"></div>
                        
                        <Avatar name={user?.fullName ?? "Bạn"} avatarUrl={user?.avatarUrl ?? null} size="sm" />

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
            <Avatar name={user?.fullName ?? "Bạn"} avatarUrl={user?.avatarUrl ?? null} size="sm" />
            
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

      {/* --- CÁC MODAL HỖ TRỢ TỪ NHÁNH MOBILE FRIENDLY --- */}
      <ReportModal
        postId={post.id}
        isOpen={showReportModal}
        onClose={() => setShowReportModal(false)}
        onSubmit={handleReport}
      />

      <ShareModal
        postId={post.id}
        isOpen={showShareModal}
        onClose={() => setShowShareModal(false)}
        onSubmit={handleShare}
      />

      {showDeleteConfirm && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-sm mx-4 shadow-lg">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Xóa bài viết?</h3>
            <p className="text-gray-600 mb-6">Bạn có chắc chắn muốn xóa bài viết này? Hành động này không thể hoàn tác.</p>
            <div className="flex gap-3 justify-end">
              <button
                onClick={() => setShowDeleteConfirm(false)}
                className="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-lg font-medium transition-colors"
              >
                Hủy
              </button>
              <button
                onClick={handleConfirmDelete}
                className="px-4 py-2 text-white bg-red-500 hover:bg-red-600 rounded-lg font-medium transition-colors"
              >
                Xóa bài viết
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default PostCard;