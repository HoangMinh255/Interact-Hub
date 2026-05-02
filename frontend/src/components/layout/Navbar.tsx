import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const Navbar = () => {
  const [search, setSearch] = useState("");
  const [showResults, setShowResults] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const { isAuthenticated, logout } = useAuth();
  const navigate = useNavigate();

  const allUsers = [
    { id: "1", name: "Carol White", type: "người dùng" },
    { id: "2", name: "Alice Johnson", type: "người dùng" },
    { id: "3", name: "Eve Davis", type: "người dùng" },
    { id: "4", name: "David Brown", type: "người dùng" },
    { id: "5", name: "Bob Smith", type: "người dùng" },
  ];

  const allHashtags = [
    { tag: "#ReactJS", count: 1200 },
    { tag: "#LapTrinh", count: 980 },
    { tag: "#SaiGon", count: 754 },
    { tag: "#CongNghe", count: 632 },
  ];

  const filteredUsers = search.trim()
    ? allUsers.filter((u) => u.name.toLowerCase().includes(search.toLowerCase()))
    : [];

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
              {!hasResults && (
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
                      <div className="w-8 h-8 rounded-full bg-blue-400 flex items-center justify-center text-white text-xs font-medium">
                        {u.name.charAt(0)}
                      </div>
                      <div>
                        <p className="text-sm text-gray-800">{u.name}</p>
                        <p className="text-xs text-gray-400">{u.type}</p>
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
              {/* Notification bell */}
              <button className="relative w-9 h-9 rounded-full bg-gray-100 flex items-center justify-center">
                🔔
                <span className="absolute top-0 right-0 w-4 h-4 bg-red-500 rounded-full text-white text-[9px] flex items-center justify-center">3</span>
              </button>

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