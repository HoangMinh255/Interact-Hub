import { Link } from "react-router-dom";

const NotFound = () => {
  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="text-center">
        <div className="text-6xl font-bold text-blue-500 mb-4">404</div>
        <h2 className="text-xl font-medium text-gray-800 mb-2">
          Trang không tồn tại
        </h2>
        <p className="text-sm text-gray-400 mb-6">
          Trang bạn tìm kiếm không tồn tại hoặc đã bị xóa
        </p>
        <Link
          to="/"
          className="px-6 py-2.5 bg-blue-500 text-white text-sm rounded-lg hover:bg-blue-600"
        >
          Về trang chủ
        </Link>
      </div>
    </div>
  );
};

export default NotFound;