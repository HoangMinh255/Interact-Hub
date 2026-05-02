import { useState } from "react";
import type { Post } from "../types";
import PostCard from "../components/PostCard";
import Sidebar from "../components/layout/Sidebar";

const Favorites = () => {
  const [favorites] = useState<Post[]>([]);

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
        <div className="hidden md:block">
          <Sidebar />
        </div>
      <div className="max-w-3xl mx-auto px-4 py-6 flex flex-col gap-4">
        <div className="bg-white border border-gray-200 rounded-xl p-4">
          <h2 className="text-base font-medium text-gray-800">❤️ Bài viết yêu thích</h2>
        </div>

        {favorites.length === 0 ? (
          <div className="bg-white border border-gray-200 rounded-xl p-10 text-center">
            <p className="text-3xl mb-3">💔</p>
            <p className="text-sm font-medium text-gray-600">Chưa có bài viết yêu thích</p>
            <p className="text-xs text-gray-400 mt-1">Bấm ❤️ vào bài viết để lưu vào đây</p>
          </div>
        ) : (
          favorites.map((post) => <PostCard key={post.id} post={post} />)
        )}
      </div>
    </div>
  );
};

export default Favorites;