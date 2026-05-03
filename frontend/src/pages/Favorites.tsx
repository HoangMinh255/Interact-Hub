import { useEffect, useState } from "react";
import type { Post } from "../types";
import PostCard from "../components/PostCard";
import Sidebar from "../components/layout/Sidebar";
import { postsAPI } from "../api";

const Favorites = () => {
  const [favorites, setFavorites] = useState<Post[]>([]);
  const [favoritesLoading, setFavoritesLoading] = useState(true);

  useEffect(() => {
    let active = true;

    const loadFavorites = async () => {
      setFavoritesLoading(true);
      try {
        const response = await postsAPI.getLiked();
        const likedPosts = Array.isArray(response.data?.data)
          ? response.data.data
          : Array.isArray(response.data)
            ? response.data
            : [];

        if (!active) return;
        setFavorites(
          likedPosts.map((post: any) => ({
            id: post.id,
            content: post.content,
            visibility: post.visibility ?? 0,
            createdAt: post.createdAt,
            authorName: post.authorName ?? "Unknown",
            authorAvatar: post.authorAvatar ?? "",
            mediaUrls: post.mediaUrls ?? [],
            likesCount: post.likeCount ?? post.likesCount ?? 0,
            commentCount: post.commentCount ?? 0,
            author: post.authorId
              ? {
                  id: post.authorId,
                  fullName: post.authorName ?? "",
                  userName: "",
                  email: "",
                  followersCount: 0,
                }
              : undefined,
          }))
        );
      } finally {
        if (active) setFavoritesLoading(false);
      }
    };

    void loadFavorites();

    return () => {
      active = false;
    };
  }, []);

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <div className="hidden md:block">
        <Sidebar />
      </div>

      <main className="flex-1 max-w-3xl mx-auto w-full">
        <div className="bg-white border border-gray-200 rounded-xl p-6 flex flex-col gap-4">
          <div>
            <h1 className="text-2xl font-semibold text-gray-800">Bài viết yêu thích</h1>
            <p className="text-sm text-gray-500 mt-1">Các bài viết bạn đã thích</p>
          </div>

          {favoritesLoading ? (
            <div className="border border-gray-200 rounded-xl p-10 text-center bg-gray-50">
              <p className="text-sm font-medium text-gray-600">Đang tải bài viết yêu thích...</p>
            </div>
          ) : favorites.length === 0 ? (
            <div className="border border-dashed border-gray-200 rounded-xl p-10 text-center bg-gray-50">
              <p className="text-3xl mb-3">💔</p>
              <p className="text-sm font-medium text-gray-600">Chưa có bài viết yêu thích</p>
              <p className="text-xs text-gray-400 mt-1">Bấm ❤️ vào bài viết để thêm vào đây</p>
            </div>
          ) : (
            <div className="flex flex-col gap-4">
              {favorites.map((post) => (
                <PostCard key={post.id} post={post} />
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
};

export default Favorites;