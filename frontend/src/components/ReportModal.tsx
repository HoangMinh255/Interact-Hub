import { useState } from "react";

interface ReportModalProps {
  postId: string;
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (reason: string) => Promise<void>;
}

function ReportModal({ postId, isOpen, onClose, onSubmit }: ReportModalProps) {
  const [reason, setReason] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!reason.trim()) {
      setError("Vui lòng nhập lý do báo cáo.");
      return;
    }

    if (reason.length > 2000) {
      setError("Lý do báo cáo không được vượt quá 2000 ký tự.");
      return;
    }

    setIsSubmitting(true);
    try {
      await onSubmit(reason.trim());
      setSuccess(true);
      setReason("");
      setTimeout(() => {
        setSuccess(false);
        onClose();
      }, 1500);
    } catch (err) {
      const errorData = (err as any)?.response?.data;
      let errorMessage = "Báo cáo thất bại. Vui lòng thử lại.";

      if (Array.isArray(errorData?.errors)) {
        errorMessage = errorData.errors[0] ?? errorData.message ?? errorMessage;
      } else if (typeof errorData?.errors === "object" && errorData?.errors !== null) {
        const errorObj = errorData.errors as Record<string, string[]>;
        const firstErrorField = Object.keys(errorObj)[0];
        if (firstErrorField && Array.isArray(errorObj[firstErrorField])) {
          errorMessage = errorObj[firstErrorField][0];
        }
      } else if (typeof errorData?.message === "string") {
        errorMessage = errorData.message;
      }

      setError(errorMessage);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-xl p-6 w-full max-w-md mx-4">
        <h2 className="text-lg font-semibold text-gray-800 mb-4">Báo cáo bài viết</h2>

        {success ? (
          <div className="text-center py-8">
            <div className="text-4xl mb-2">✓</div>
            <p className="text-green-600 font-medium">Báo cáo đã được gửi thành công!</p>
          </div>
        ) : (
          <form onSubmit={handleSubmit}>
            <div className="mb-4">
              <label className="text-sm text-gray-600 mb-2 block">Lý do báo cáo *</label>
              <textarea
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                placeholder="Vui lòng mô tả lý do bạn muốn báo cáo bài viết này..."
                className="w-full h-32 border border-gray-200 rounded-lg px-3 py-2 text-sm resize-none outline-none focus:border-blue-400"
                disabled={isSubmitting}
              />
              <div className="text-xs text-gray-400 mt-1">
                {reason.length}/2000
              </div>
            </div>

            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
                <p className="text-xs text-red-600">{error}</p>
              </div>
            )}

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
                className="flex-1 px-4 py-2 text-sm font-medium text-white bg-red-500 rounded-lg hover:bg-red-600 disabled:opacity-50"
                disabled={isSubmitting}
              >
                {isSubmitting ? "Đang gửi..." : "Gửi báo cáo"}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export default ReportModal;
