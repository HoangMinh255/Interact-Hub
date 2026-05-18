import { NavLink } from "react-router-dom";
import { IoMdSettings } from "react-icons/io";
import { FaHeart, FaUserFriends } from "react-icons/fa";
import { FcSearch, FcHome } from "react-icons/fc";
import { PiFlagPennantFill } from "react-icons/pi";

const Sidebar = () => {
  return (
    <aside className="w-full md:w-48 flex flex-col gap-1 pt-2">
      <NavLink
        to="/"
        className={({ isActive }) =>
          `flex items-center gap-3 px-3 py-2 rounded-lg text-sm ${
            isActive ? "bg-blue-50 text-blue-500 font-medium" : "text-gray-600 hover:bg-gray-100"
          }`
        }
      >
        <span className="w-7 h-7 rounded-md bg-yellow-100 flex items-center justify-center"><FcHome /></span>
        Tin tức
      </NavLink>

      <NavLink
        to="/reports"
        className={({ isActive }) =>
          `flex items-center gap-3 px-3 py-2 rounded-lg text-sm ${
            isActive ? "bg-blue-50 text-blue-500 font-medium" : "text-gray-600 hover:bg-gray-100"
          }`
        }
      >
        <span className="w-7 h-7 rounded-md bg-orange-100 flex items-center justify-center"><PiFlagPennantFill className="text-red-500" /></span>
        Báo cáo
      </NavLink>

      <NavLink
        to="/favorites"
        className={({ isActive }) =>
          `flex items-center gap-3 px-3 py-2 rounded-lg text-sm ${
            isActive ? "bg-blue-50 text-blue-500 font-medium" : "text-gray-600 hover:bg-gray-100"
          }`
        }
      >
        <span className="w-7 h-7 rounded-md bg-pink-100 flex items-center justify-center"><FaHeart className="text-red-500"/></span>
        Yêu thích
      </NavLink>

      <NavLink
        to="/friends"
        className={({ isActive }) =>
          `flex items-center gap-3 px-3 py-2 rounded-lg text-sm ${
            isActive ? "bg-blue-50 text-blue-500 font-medium" : "text-gray-600 hover:bg-gray-100"
          }`
        }
      >
        <span className="w-7 h-7 rounded-md bg-indigo-100 flex items-center justify-center"><FaUserFriends className="text-blue-500"/></span>
        Bạn bè
      </NavLink>

      <p className="text-xs text-gray-400 px-3 pt-3 pb-1">Trang</p>

      <NavLink
        to="/settings"
        className={({ isActive }) =>
          `flex items-center gap-3 px-3 py-2 rounded-lg text-sm ${
            isActive ? "bg-blue-50 text-blue-500 font-medium" : "text-gray-600 hover:bg-gray-100"
          }`
        }
      >
        <span className="w-7 h-7 rounded-md bg-gray-100 flex items-center justify-center"><IoMdSettings className="text-grey-500"/></span>
        Cài đặt
      </NavLink>
    </aside>
  );
};

export default Sidebar;