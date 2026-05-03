import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
// 1. Import thêm notificationsAPI cùng với usersAPI
import { usersAPI, notificationsAPI } from "../../api"; 

import * as signalR from "@microsoft/signalr";

type SearchUser = {
  id: string;
  fullName: string;
  userName: string;
  avatarUrl?: string | null;
  bio?: string | null;
};

type Notification = {
  id: string;
  content: string;
  isRead: boolean;
  createdAt: string;
  actorId?: string;
  recipientId?: string;
};

const Navbar = () => {
  const [search, setSearch] = useState("");
  const [showResults, setShowResults] = useState(false);
  const [searchedUsers, setSearchedUsers] = useState<SearchUser[]>([]);
  const [searchLoading, setSearchLoading] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);

  //Khai báo State cho Thông báo
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [showNotifications, setShowNotifications] = useState(false);
  const [unreadCount, setUnreadCount] = useState(0);
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const allHashtags = [
    { tag: "#ReactJS", count: 1200 },
    { tag: "#LapTrinh", count: 980 },
    { tag: "#SaiGon", count: 754 },
    { tag: "#CongNghe", count: 632 },
  ];

  // --- LOGIC TÌM KIẾM ---
  useEffect(() => {
    const keyword = search.trim();

    if (!keyword) {
      setSearchedUsers([]);
      setSearchLoading(false);
      return;
    }

    let active = true;
    setSearchLoading(true);

    const timer = window.setTimeout(() => {
      usersAPI.searchUsers(keyword)
        .then((response) => {
          if (!active) return;

          const items = response.data?.data?.items ?? [];
          setSearchedUsers(items);
        })
        .catch(() => {
          if (!active) return;
          setSearchedUsers([]);
        })
        .finally(() => {
          if (!active) return;
          setSearchLoading(false);
        });
    }, 300);

    return () => {
      active = false;
      window.clearTimeout(timer);
    };
  }, [search]);

  // --- LOGIC THÔNG BÁO (SIGNALR & API) ---
  useEffect(() => {
    if (!isAuthenticated) return;

    // 2. Viết hàm lấy thông báo cũ bằng async/await giống Home.tsx
    const fetchNotifications = async () => {
      try {
        const response = await notificationsAPI.getAll(); 
        // Tuỳ thuộc vào cấu trúc trả về của API, thông thường sẽ là response.data
        setNotifications(response.data || []);
      } catch (error: any) {
        console.error("Lỗi tải thông báo:", error.response?.data?.message || error.message);
      }
    };

    // Gọi hàm fetch
    fetchNotifications();
    
    // Khởi tạo kết nối SignalR
    const connection = new signalR.HubConnectionBuilder()
      // Thay bằng URL Backend thực tế của bạn
      .withUrl("https://localhost:5226/notificationHub", {
        // Lấy token từ nơi bạn lưu (localStorage, cookie, hoặc AuthContext)
        accessTokenFactory: () => localStorage.getItem("token") || "" 
      })
      .withAutomaticReconnect()
      .build();

    // Lắng nghe sự kiện từ Backend (Tên sự kiện phải khớp 100% với C# SendAsync)
    connection.on("ReceiveNewNotification", (newNotif: Notification) => {
      // Cập nhật danh sách: đưa thông báo mới lên đầu
      setNotifications((prev) => [newNotif, ...prev]);
    });

    // Bắt đầu kết nối
    connection.start()
      .then(() => console.log("Đã kết nối SignalR Notification!"))
      .catch((err) => console.error("Lỗi kết nối SignalR: ", err));

    // Cleanup khi component unmount
    return () => {
      connection.stop();
    };
  }, [isAuthenticated]);

  // Đếm số lượng chưa đọc mỗi khi mảng notifications thay đổi
  useEffect(() => {
    const unread = notifications.filter((n) => !n.isRead).length;
    setUnreadCount(unread);
  }, [notifications]);

  // 3. Sửa lại hàm này thành async để gọi API cập nhật trạng thái "Đã đọc"
  const handleReadNotification = async (id: string) => {
    // Cập nhật state nội bộ trước (Optimistic UI - cho người dùng thấy phản hồi ngay)
    setNotifications((prev) =>
      prev.map((n) => (n.id === id ? { ...n, isRead: true } : n))
    );
    
    try {
      // Gọi API PUT để báo cho Backend cập nhật Database
      // Lưu ý: Đảm bảo trong file api.ts bạn có định nghĩa method update cho notifications
      await notificationsAPI.update(id); 
    } catch (error: any) {
      console.error("Lỗi đánh dấu đã đọc:", error.response?.data?.message || error.message);
      // Tùy chọn: Nếu lỗi, có thể hoàn tác lại trạng thái isRead = false tại đây
    }

    // Đóng popup nếu cần hoặc điều hướng
    setShowNotifications(false);
  };

  const filteredUsers = searchedUsers;

  const filteredTags = search.trim()
    ? allHashtags.filter((h) => h.tag.toLowerCase().includes(search.toLowerCase()))
    : [];

  const hasResults = filteredUsers.length > 0 || filteredTags.length > 0;

  const handleLogout = () => {
    logout();
    setMenuOpen(false);
    navigate("/login");
  };

  return (
    <nav className="bg-white border-b border-gray-200 sticky top-0 z-10">
      {/* Main bar */}
      <div className="px-4 h-14 flex items-center gap-3">

        {/* Logo */}
        <Link to="/" className="flex items-center gap-2 shrink-0">
          <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">
            IH
          </div>
          <span className="font-medium text-gray-800 hidden sm:block">InteractHub</span>
        </Link>

        {/* Search — hidden on mobile, visible md+ */}
        <div className="relative flex-1 max-w-sm hidden md:block">
          <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">🔍</span>
          <input
            type="text"
            placeholder="Tìm kiếm bài viết, bạn bè ..."
            value={search}
            onChange={(e) => { setSearch(e.target.value); setShowResults(true); }}
            onFocus={() => setShowResults(true)}
            onBlur={() => setTimeout(() => setShowResults(false), 200)}
            className="w-full h-9 bg-gray-100 rounded-full pl-9 pr-4 text-sm outline-none"
          />
          {showResults && search.trim() && (
            <div className="absolute top-11 left-0 w-72 bg-white border border-gray-200 rounded-xl shadow-sm overflow-hidden z-20">
              {searchLoading && (
                <div className="px-4 py-3 text-sm text-gray-400">Đang tìm kiếm...</div>
              )}
              {!searchLoading && !hasResults && (
                <div className="px-4 py-3 text-sm text-gray-400">Không tìm thấy kết quả</div>
              )}
              {filteredUsers.length > 0 && (
                <div>
                  <p className="text-xs text-gray-400 px-4 pt-3 pb-1">Người dùng</p>
                  {filteredUsers.map((u) => (
                    <button
                      key={u.id}
                      onClick={() => { navigate(`/profile/${u.id}`); setSearch(""); setShowResults(false); }}
                      className="w-full flex items-center gap-3 px-4 py-2 hover:bg-gray-50 text-left"
                    >
                        <div className="w-8 h-8 rounded-full bg-blue-400 flex items-center justify-center text-white text-xs font-medium overflow-hidden">
                          {u.avatarUrl ? (
                            <img src={u.avatarUrl} alt={u.fullName} className="w-full h-full object-cover" />
                          ) : (
                            (u.fullName || u.userName).charAt(0).toUpperCase()
                          )}
                      </div>
                      <div>
                          <p className="text-sm text-gray-800">{u.fullName}</p>
                          <p className="text-xs text-gray-400">@{u.userName}</p>
                      </div>
                    </button>
                  ))}
                </div>
              )}
              {filteredTags.length > 0 && (
                <div>
                  <p className="text-xs text-gray-400 px-4 pt-3 pb-1">Hashtag</p>
                  {filteredTags.map((h) => (
                    <button
                      key={h.tag}
                      onClick={() => { navigate(`/?tag=${h.tag}`); setSearch(""); setShowResults(false); }}
                      className="w-full flex items-center gap-3 px-4 py-2 hover:bg-gray-50 text-left"
                    >
                      <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-500 text-xs font-medium">
                        #
                      </div>
                      <div>
                        <p className="text-sm text-blue-500">{h.tag}</p>
                        <p className="text-xs text-gray-400">{h.count.toLocaleString()} bài viết</p>
                      </div>
                    </button>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>

        {/* Right side actions */}
        <div className="ml-auto flex items-center gap-2">

          {/* Mobile search icon — shown only on mobile */}
          <button
            className="w-9 h-9 rounded-full bg-gray-100 flex items-center justify-center md:hidden"
            onClick={() => setMenuOpen((prev) => !prev)}
          >
            🔍
          </button>

          {isAuthenticated ? (
            <>
              {/* Vùng Thông báo */}
              <div className="relative">
                <button 
                    className="relative w-9 h-9 rounded-full bg-gray-100 flex items-center justify-center hover:bg-gray-200 transition-colors"
                    onClick={() => setShowNotifications((prev) => !prev)}
                  >
                    🔔
                    {unreadCount > 0 && (
                      <span className="absolute -top-1 -right-1 w-4 h-4 bg-red-500 rounded-full text-white text-[10px] font-bold flex items-center justify-center border-2 border-white">
                        {unreadCount > 99 ? '99+' : unreadCount}
                      </span>
                    )}
                </button>

                {/* Dropdown hiển thị danh sách Thông báo (Bạn nhớ bổ sung HTML phần này nếu lỡ xóa) */}
                {showNotifications && (
                  <div className="absolute right-0 top-12 w-80 bg-white border border-gray-200 rounded-xl shadow-lg z-50 overflow-hidden">
                    <div className="px-4 py-3 border-b border-gray-100 bg-gray-50">
                      <h3 className="font-semibold text-gray-800">Thông báo</h3>
                    </div>
                    <div className="max-h-80 overflow-y-auto">
                      {notifications.length === 0 ? (
                        <div className="px-4 py-8 text-center text-sm text-gray-500">
                          Bạn chưa có thông báo nào.
                        </div>
                      ) : (
                        notifications.map((notif) => (
                          <button
                            key={notif.id}
                            onClick={() => handleReadNotification(notif.id)}
                            className={`w-full text-left px-4 py-3 flex gap-3 hover:bg-gray-50 transition-colors border-b border-gray-50 last:border-0 ${
                              !notif.isRead ? 'bg-blue-50/30' : ''
                            }`}
                          >
                            <div className="w-10 h-10 shrink-0 rounded-full bg-blue-100 flex items-center justify-center text-xl">
                              💡
                            </div>
                            <div className="flex-1">
                              <p className={`text-sm text-gray-800 ${!notif.isRead ? 'font-semibold' : 'font-normal'}`}>
                                {notif.content}
                              </p>
                            </div>
                            {!notif.isRead && (
                              <div className="w-2 h-2 rounded-full bg-blue-500 mt-2 shrink-0"></div>
                            )}
                          </button>
                        ))
                      )}
                    </div>
                  </div>
                )}
              </div>

              {/* Avatar */}
              <Link
                to="/profile"
                className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-xs font-medium cursor-pointer"
              >
                U
              </Link>

              {/* Logout — hidden on mobile (accessible via hamburger) */}
              <button
                onClick={handleLogout}
                className="text-sm text-gray-600 hover:text-red-500 hidden sm:block"
              >
                Đăng xuất
              </button>

              {/* Hamburger — mobile only */}
              <button
                className="w-9 h-9 rounded-full bg-gray-100 flex items-center justify-center sm:hidden"
                onClick={() => setMenuOpen((prev) => !prev)}
              >
                {menuOpen ? "✕" : "☰"}
              </button>
            </>
          ) : (
            <Link to="/login" className="text-sm bg-blue-500 text-white px-4 py-1.5 rounded-full">
              Đăng nhập
            </Link>
          )}
        </div>
      </div>

      {/* Mobile dropdown menu */}
      {menuOpen && (
        <div className="border-t border-gray-100 px-4 py-3 flex flex-col gap-3 sm:hidden bg-white">

          {/* Mobile search bar */}
          <div className="relative">
            <span className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">🔍</span>
            <input
              type="text"
              placeholder="Tìm kiếm..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full h-9 bg-gray-100 rounded-full pl-9 pr-4 text-sm outline-none"
            />
          </div>

          {/* Mobile nav links */}
          <Link
            to="/"
            onClick={() => setMenuOpen(false)}
            className="flex items-center gap-3 py-2 text-sm text-gray-700 hover:text-blue-500"
          >
            🏠 Tin tức
          </Link>
          <Link
            to="/friends"
            onClick={() => setMenuOpen(false)}
            className="flex items-center gap-3 py-2 text-sm text-gray-700 hover:text-blue-500"
          >
            👥 Bạn bè
          </Link>
          <Link
            to="/favorites"
            onClick={() => setMenuOpen(false)}
            className="flex items-center gap-3 py-2 text-sm text-gray-700 hover:text-blue-500"
          >
            ❤️ Yêu thích
          </Link>
          <Link
            to="/settings"
            onClick={() => setMenuOpen(false)}
            className="flex items-center gap-3 py-2 text-sm text-gray-700 hover:text-blue-500"
          >
            ⚙️ Cài đặt
          </Link>

          {/* Logout in mobile menu */}
          {isAuthenticated && (
            <button
              onClick={handleLogout}
              className="text-left py-2 text-sm text-red-500"
            >
              🚪 Đăng xuất
            </button>
          )}
        </div>
      )}
    </nav>
  );
};

export default Navbar;