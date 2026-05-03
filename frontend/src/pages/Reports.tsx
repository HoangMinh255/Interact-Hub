import { useEffect, useState } from "react";
import Sidebar from "../components/layout/Sidebar";
import { reportsAPI } from "../api";
import LoadingSkeleton from "../components/ui/LoadingSkeleton";

interface Report {
  id: string;
  postId: string;
  reporterId: string;
  reporterName?: string;
  reason: string;
  status: number | string; // Can be number (0-3) or string ("Pending", "Reviewed", etc.)
  reviewedById?: string;
  reviewedByName?: string;
  reviewedAt?: string;
  createdAt: string;
}

const getStatusLabel = (status: number | string) => {
  // Handle string status values (enum names)
  if (typeof status === "string") {
    const statusMap: Record<string, string> = {
      "Pending": "Chờ xử lý",
      "Reviewed": "Đã xem xét",
      "Rejected": "Bị từ chối",
      "Resolved": "Đã giải quyết"
    };
    return statusMap[status] || "Không xác định";
  }

  // Handle numeric status values
  switch (status) {
    case 0:
      return "Chờ xử lý";
    case 1:
      return "Đã xem xét";
    case 2:
      return "Bị từ chối";
    case 3:
      return "Đã giải quyết";
    default:
      return "Không xác định";
  }
};

const getStatusColor = (status: number | string) => {
  // Normalize string status to number
  let statusNum = status;
  if (typeof status === "string") {
    const statusMap: Record<string, number> = {
      "Pending": 0,
      "Reviewed": 1,
      "Rejected": 2,
      "Resolved": 3
    };
    statusNum = statusMap[status] ?? -1;
  }

  switch (statusNum) {
    case 0:
      return "bg-yellow-50 text-yellow-700 border-yellow-200";
    case 1:
      return "bg-blue-50 text-blue-700 border-blue-200";
    case 2:
      return "bg-red-50 text-red-700 border-red-200";
    case 3:
      return "bg-green-50 text-green-700 border-green-200";
    default:
      return "bg-gray-50 text-gray-700 border-gray-200";
  }
};

function Reports() {
  const [reports, setReports] = useState<Report[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  useEffect(() => {
    const loadReports = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await reportsAPI.getMyReports(page, 10);
        
        // Handle paginated response
        const data = response.data?.data;
        if (data) {
          // Debug: Log the actual status values
          console.log("Reports response:", data);
          data.items?.forEach((item: any) => {
            console.log(`Report ${item.id}: status = ${item.status} (type: ${typeof item.status})`);
          });
          
          setReports(data.items || []);
          setTotalPages(data.totalPages || 1);
        }
      } catch (err: any) {
        const errorMsg = err.response?.data?.message || "Không thể tải báo cáo";
        setError(errorMsg);
        setReports([]);
      } finally {
        setLoading(false);
      }
    };

    void loadReports();
  }, [page]);

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <div className="hidden md:block">
        <Sidebar />
      </div>

      <main className="flex-1">
        <div className="bg-white border border-gray-200 rounded-xl p-6">
          <h1 className="text-2xl font-semibold text-gray-800 mb-2">Báo cáo của tôi</h1>
          <p className="text-sm text-gray-500 mb-6">Quản lý danh sách báo cáo bạn đã gửi</p>

          {error && (
            <div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
              <p className="text-sm text-red-600">{error}</p>
            </div>
          )}

          {loading ? (
            <div className="space-y-3">
              <LoadingSkeleton />
              <LoadingSkeleton />
              <LoadingSkeleton />
            </div>
          ) : reports.length === 0 ? (
            <div className="text-center py-12">
              <div className="text-4xl mb-3">🚩</div>
              <p className="text-gray-500">Bạn chưa gửi báo cáo nào</p>
            </div>
          ) : (
            <div className="space-y-3">
              {reports.map((report) => (
                <div
                  key={report.id}
                  className="border border-gray-200 rounded-lg p-4 hover:bg-gray-50 transition"
                >
                  <div className="flex items-start justify-between mb-3">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-2">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${getStatusColor(report.status)}`}>
                          {getStatusLabel(report.status)}
                        </span>
                        <span className="text-xs text-gray-400">
                          Bài viết ID: {report.postId.substring(0, 8)}...
                        </span>
                      </div>
                      <p className="text-sm text-gray-700 mb-2">{report.reason}</p>
                      <div className="flex items-center gap-4 text-xs text-gray-500">
                        <span>📅 {new Date(report.createdAt).toLocaleDateString("vi-VN")}</span>
                        {report.reviewedAt && (
                          <span>✓ Xem xét: {new Date(report.reviewedAt).toLocaleDateString("vi-VN")}</span>
                        )}
                        {report.reviewedByName && (
                          <span>👤 {report.reviewedByName}</span>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Pagination */}
          {totalPages > 1 && !loading && (
            <div className="flex items-center justify-center gap-2 mt-6 pt-4 border-t border-gray-200">
              <button
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page === 1}
                className="px-3 py-2 text-sm text-gray-600 border border-gray-200 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                ← Trước
              </button>

              <div className="flex items-center gap-1">
                {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                  const pageNum = page > 3 ? page - 2 + i : i + 1;
                  return (
                    pageNum <= totalPages && (
                      <button
                        key={pageNum}
                        onClick={() => setPage(pageNum)}
                        className={`w-8 h-8 flex items-center justify-center rounded text-sm ${
                          pageNum === page
                            ? "bg-blue-500 text-white font-medium"
                            : "border border-gray-200 text-gray-600 hover:bg-gray-50"
                        }`}
                      >
                        {pageNum}
                      </button>
                    )
                  );
                })}
              </div>

              <button
                onClick={() => setPage(Math.min(totalPages, page + 1))}
                disabled={page === totalPages}
                className="px-3 py-2 text-sm text-gray-600 border border-gray-200 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Sau →
              </button>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

export default Reports;
