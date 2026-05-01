import { useEffect, useState } from "react";
import { commentsAPI } from "../api";
import type { Post } from "../types";

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

function PostCard({ post, onDelete }: PostCardProps) {
  const [liked, setLiked] = useState(false);
  const [likes, setLikes] = useState(0);
  const [showComment, setShowComment] = useState(false);
  const [comment, setComment] = useState("");
  const [comments, setComments] = useState<CommentItem[]>([]);
  const [localComments, setLocalComments] = useState<CommentItem[]>([]);

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

  const handleLike = () => {
    setLiked(!liked);
    setLikes(liked ? likes - 1 : likes + 1);
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

  return (
    <div className="bg-white border border-gray-200 rounded-xl p-4">
      <div className="flex items-center gap-3 mb-3">
        {post.authorAvatar ? (
          <img
            src={post.authorAvatar}
            alt={post.authorName || "Author avatar"}
            className="w-9 h-9 rounded-full object-cover border border-gray-200"
            referrerPolicy="no-referrer"
          />
        ) : (
          <div className="w-9 h-9 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">
            {(post.authorName || "U").charAt(0).toUpperCase()}
          </div>
        )}
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-800">{post.authorName || "Unknown"}</p>
          <p className="text-xs text-gray-400">{post.createdAt}</p>
        </div>
        {onDelete && (
          <button
            onClick={onDelete}
            className="text-xs text-red-400 hover:text-red-600 px-2 py-1 rounded hover:bg-red-50"
          >
            🗑️ Xóa
          </button>
        )}
      </div>

      <p className="text-sm text-gray-700 mb-3">{post.content}</p>

      {post.mediaUrls && post.mediaUrls.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 mb-3">
          {post.mediaUrls.map((url, idx) => (
            <img key={idx} src={url} alt="media" className="w-full rounded-lg object-cover max-h-64" />
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
          onClick={() => alert("Đã chia sẻ bài viết!")}
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
    </div>
  );
}

export default PostCard;