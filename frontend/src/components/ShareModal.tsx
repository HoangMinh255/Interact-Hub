import { useState } from "react";

interface ShareModalProps {
  postId: string;
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (comment?: string) => Promise<void>;
}

function ShareModal({ postId, isOpen, onClose, onSubmit }: ShareModalProps) {
  void postId;
  const [comment, setComment] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  if (!isOpen) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await onSubmit(comment.trim() || undefined);
      setSuccess(true);
      setComment("");
      setTimeout(() => {
        setSuccess(false);
        onClose();
      }, 1200);
    } catch (err) {
      const errorData = (err as any)?.response?.data;
      setError(typeof errorData?.message === "string" ? errorData.message : "Không thể chia sẻ bài viết. Vui lòng thử lại.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl p-6 w-full max-w-md mx-4">
        <h2 className="text-lg font-semibold text-gray-800 mb-4">Chia sẻ bài viết</h2>

        {success ? (
          <div className="text-center py-8">
            <div className="text-4xl mb-2">✓</div>
            <p className="text-green-600 font-medium">Bài viết đã được chia sẻ!</p>
          </div>
        ) : (
          <form onSubmit={handleSubmit}>
            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
                <p className="text-xs text-red-600">{error}</p>
              </div>
            )}
            <div className="mb-4">
              <label className="text-sm text-gray-600 mb-2 block">Thêm bình luận (không bắt buộc)</label>
              <textarea
                value={comment}
                onChange={(e) => setComment(e.target.value)}
                placeholder="Thêm dòng chú thích khi chia sẻ..."
                className="w-full h-24 border border-gray-200 rounded-lg px-3 py-2 text-sm resize-none outline-none focus:border-blue-400"
                disabled={isSubmitting}
              />
            </div>

            <div className="flex gap-2">
              <button
                type="button"
                onClick={onClose}
                className="flex-1 px-4 py-2 text-sm font-medium text-gray-600 bg-gray-100 rounded-lg hover:bg-gray-200 disabled:opacity-50"
                disabled={isSubmitting}
              >
                Hủy
              </button>
              <button
                type="submit"
                className="flex-1 px-4 py-2 text-sm font-medium text-white bg-blue-500 rounded-lg hover:bg-blue-600 disabled:opacity-50"
                disabled={isSubmitting}
              >
                {isSubmitting ? "Đang chia sẻ..." : "Chia sẻ"}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export default ShareModal;
