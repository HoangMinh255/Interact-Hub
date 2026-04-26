import { NavLink } from "react-router-dom";

const BottomNav = () => {
  return (
    <nav className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 flex md:hidden z-10">
      <NavLink to="/" className={({ isActive }) =>
        `flex-1 flex flex-col items-center py-2 text-xs ${isActive ? "text-blue-500" : "text-gray-500"}`
      }>
        <span className="text-lg">🏠</span>
        Tin tức
      </NavLink>
      <NavLink to="/friends" className={({ isActive }) =>
        `flex-1 flex flex-col items-center py-2 text-xs ${isActive ? "text-blue-500" : "text-gray-500"}`
      }>
        <span className="text-lg">👥</span>
        Bạn bè
      </NavLink>
      <NavLink to="/favorites" className={({ isActive }) =>
        `flex-1 flex flex-col items-center py-2 text-xs ${isActive ? "text-blue-500" : "text-gray-500"}`
      }>
        <span className="text-lg">❤️</span>
        Yêu thích
      </NavLink>
      <NavLink to="/profile" className={({ isActive }) =>
        `flex-1 flex flex-col items-center py-2 text-xs ${isActive ? "text-blue-500" : "text-gray-500"}`
      }>
        <span className="text-lg">👤</span>
        Hồ sơ
      </NavLink>
      <NavLink to="/settings" className={({ isActive }) =>
        `flex-1 flex flex-col items-center py-2 text-xs ${isActive ? "text-blue-500" : "text-gray-500"}`
      }>
        <span className="text-lg">⚙️</span>
        Cài đặt
      </NavLink>
    </nav>
  );
};

export default BottomNav;