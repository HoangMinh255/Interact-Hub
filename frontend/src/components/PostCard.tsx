import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { commentsAPI, reportsAPI, postsAPI } from "../api";
import type { Post } from "../types";
import ReportModal from "./ReportModal";
import ShareModal from "./ShareModal";

interface CommentItem {
  id: string;
  content: string;
  authorName?: string;
  authorAvatar?: string;
  createdAt?: string;
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
  const [liked, setLiked] = useState(false);
  const [likes, setLikes] = useState(0);
  const [showComment, setShowComment] = useState(false);
  const [comment, setComment] = useState("");
  const [comments, setComments] = useState<CommentItem[]>([]);
  const [localComments, setLocalComments] = useState<CommentItem[]>([]);
  const [showReportModal, setShowReportModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [showShareModal, setShowShareModal] = useState(false);

  useEffect(() => {
    if (!showComment) return;

    const loadComments = async () => {
      try {
        const response = await commentsAPI.getByPostId(post.id);
        setComments(Array.isArray(response.data) ? response.data : []);
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
        // ignore errors and keep defaults
      }
    };

    void loadLikeState();
    return () => {
      mounted = false;
    };
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
      // Optional: show toast / error
      console.error("Like action failed", e);
    }
  };

  const handleComment = async () => {
    if (!comment.trim()) return;

    await commentsAPI.create(post.id, comment.trim());

    const newComment: CommentItem = {
      id: Date.now().toString(),
      content: comment.trim(),
      authorName: "Bạn",
    };

    setLocalComments((prev) => [...prev, newComment]);
    setComment("");
  };

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

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4">
      <div className="flex items-center gap-3 mb-3">
        {(() => {
          // Use sharer's avatar if this is a shared post, otherwise use original author's avatar
          let avatar = post.isShared ? (post.sharedByAvatar ?? "") : (post.authorAvatar ?? "");
          let displayName = post.isShared ? (post.sharedByName ?? "Unknown") : (post.authorName || "Unknown");
          let targetId = post.isShared 
            ? (post.sharedById ?? null) 
            : (post.author?.id ?? (post as any).authorId ?? null);

          if (targetId) {
            return (
              <Link to={`/profile/${targetId}`} className="shrink-0">
                {avatar ? (
                  <img src={avatar} alt={displayName} className="w-9 h-9 rounded-full object-cover border border-gray-200" referrerPolicy="no-referrer" />
                ) : (
                  <div className="w-9 h-9 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">{(displayName || "U").charAt(0).toUpperCase()}</div>
                )}
              </Link>
            );
          }

          return avatar ? (
            <img src={avatar} alt={displayName} className="w-9 h-9 rounded-full object-cover border border-gray-200" referrerPolicy="no-referrer" />
          ) : (
            <div className="w-9 h-9 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">{(displayName || "U").charAt(0).toUpperCase()}</div>
          );
        })()}
        <div className="flex-1">
          {(() => {
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
            <span className="inline-flex items-center rounded-full bg-gray-100 px-2 py-0.5 text-[11px] font-medium text-gray-500">
              {getVisibilityLabel(post.visibility)}
            </span>
          </div>
        </div>
        {onDelete && (
          <button
            onClick={() => setShowDeleteConfirm(true)}
            className="text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50"
          >
            🗑️
          </button>
        )}
        {!onDelete && (
          <button
            onClick={() => setShowReportModal(true)}
            className="text-sm text-gray-500 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50 transition-colors"
            title="Báo cáo bài viết"
          >
            🚩
          </button>
        )}
      </div>

      {post.isShared && post.shareComment && (
        <div className="mb-3 pb-3 border-b border-gray-200">
          <p className="text-sm text-gray-600 italic bg-blue-50 rounded-lg px-3 py-2 border-l-2 border-blue-400">
            "{post.shareComment}"
          </p>
        </div>
      )}

      <p className="text-sm text-gray-700 mb-3">{post.content}</p>

      {post.mediaUrls && post.mediaUrls.length > 0 && (
        <div className="flex flex-col gap-2 mb-3">
          {post.mediaUrls.map((url, idx) => (
            <img key={idx} src={url} alt="media" className="block w-full h-auto rounded-lg" />
          ))}
        </div>
      )}

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
          💬 {(post.commentCount || 0) + localComments.length} Bình luận
        </button>
        <button
          onClick={() => setShowShareModal(true)}
          className="flex-1 flex items-center justify-center gap-1 py-1.5 text-sm text-gray-500 hover:bg-gray-50 rounded-lg"
        >
          🔁 Chia sẻ
        </button>
      </div>

      {showComment && (
        <div className="mt-3 border-t border-gray-100 pt-3">
          {[...comments, ...localComments].map((c) => (
            <div key={c.id} className="flex gap-2 mb-2">
              <div className="w-7 h-7 rounded-full bg-blue-400 flex items-center justify-center text-white text-xs">
                {(c.authorName ?? "U").charAt(0).toUpperCase()}
              </div>
              <div className="bg-gray-100 rounded-lg px-3 py-1.5 text-sm text-gray-700">
                <p>{c.content}</p>
              </div>
            </div>
          ))}
          <div className="flex gap-2 mt-2">
            <div className="w-7 h-7 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs">U</div>
            <input
              type="text"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleComment()}
              placeholder="Viết bình luận..."
              className="flex-1 bg-gray-100 rounded-full px-3 text-sm outline-none h-8"
            />
            <button onClick={() => void handleComment()} className="text-blue-500 text-sm font-medium">Gửi</button>
          </div>
        </div>
      )}

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