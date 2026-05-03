/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Sidebar from "../components/layout/Sidebar";
import PostCard from "../components/PostCard";
import LoadingSkeleton from "../components/ui/LoadingSkeleton";
import { usePosts } from "../hooks/usePosts";
import { postsAPI } from "../api";
import type { Post } from "../types";
import { useAuth } from "../context/AuthContext";

function Home() {
  const { posts: initialPosts, loading } = usePosts();
  const [posts, setPosts] = useState<Post[]>([]);
  const [newPost, setNewPost] = useState("");
  const [visibility, setVisibility] = useState(0);
  const [activeTag, setActiveTag] = useState<string | null>(null);
  const fileRef = useRef<HTMLInputElement>(null);
  const navigate = useNavigate();
  const [selectedMedia, setSelectedMedia] = useState<Array<{url: string, type: number}>>([]);
  const { user } = useAuth();

  useEffect(() => {
    if (initialPosts.length > 0 && posts.length === 0) {
      setPosts(initialPosts);
    }
  }, [initialPosts, posts.length]);

  const handleImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      const maxFiles = 4;
      const arr = Array.from(files).slice(0, maxFiles - selectedMedia.length);
      arr.forEach((file) => {
        const reader = new FileReader();
        reader.onloadend = () => {
          setSelectedMedia((prev) => [...prev, { url: reader.result as string, type: 0 }]);
        };
        reader.readAsDataURL(file);
      });
    }
    if (e.target) e.target.value = "";
  };

  const removeSelectedMedia = (index: number) => {
    setSelectedMedia((prev) => prev.filter((_, i) => i !== index));
  };

  const handlePost = async () => {
    if (!newPost.trim() && selectedMedia.length === 0) return;
    const mediaPayload = selectedMedia.map((m) => ({
      MediaUrl: m.url,
      MediaType: m.type,
    }));
    try {
      const response = await postsAPI.create(newPost.trim(), visibility, mediaPayload);
      console.log("Post created:", response.data);
      setNewPost("");
      setVisibility(0);
      setSelectedMedia([]);
      // Reload posts to get fresh data with author info
      const freshResponse = await postsAPI.getAll();
      const mappedPosts = Array.isArray(freshResponse.data)
        ? freshResponse.data.map((p: any) => ({
            id: p.id,
            visibility: p.visibility,
            authorName: p.authorName ?? "Unknown",
            authorAvatar: p.authorAvatar ?? "",
            author: p.authorId ? { id: p.authorId, fullName: p.authorName ?? "", userName: "", email: "", followersCount: 0 } : undefined,
            content: p.content,
            mediaUrls: p.mediaUrls ?? [],
            likesCount: 0,
            commentCount: p.commentCount ?? 0,
            createdAt: new Date(p.createdAt).toLocaleString("vi-VN"),
            isShared: p.isShared ?? false,
            shareComment: p.shareComment,
            sharedById: p.sharedById,
            sharedByName: p.sharedByName,
            sharedByAvatar: p.sharedByAvatar ?? undefined,
            originalPostId: p.originalPostId,
          }))
        : [];
      setPosts(mappedPosts);
    } catch (error: any) {
      //const errorMsg = error.response?.data?.message || error.message || "Đăng bài thất bại";
      //console.error("Post creation error:", errorMsg);
      //alert(`❌ Lỗi: ${errorMsg}`);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await postsAPI.delete(id);
      setPosts(posts.filter((p) => p.id !== id));
      alert("✅ Bài viết đã được xóa");
    } catch (error: any) {
      const errorMsg = error.response?.data?.message || error.message || "Xóa bài viết thất bại";
      console.error("Post deletion error:", errorMsg);
      alert(`❌ Lỗi: ${errorMsg}`);
    }
  };

  const filteredPosts = activeTag
    ? posts.filter((p) => p.content.includes(activeTag))
    : posts;

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <div className="hidden md:block">
        <Sidebar />
      </div>
      <main className="flex-1 flex flex-col gap-3">
        <div className="bg-white border border-gray-200 rounded-xl p-3">
          <div className="flex items-center gap-3 mb-2">
            <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm">B</div>
            <input
              type="text"
              value={newPost}
              onChange={(e) => setNewPost(e.target.value)}
              placeholder="Bạn đang nghĩ gì?"
              className="flex-1 h-9 bg-gray-100 rounded-full px-4 text-sm outline-none"
            />
          </div>

          {selectedMedia.length > 0 && (
            <div className="mb-2 ml-12 grid grid-cols-1 sm:grid-cols-2 gap-2">
              {selectedMedia.map((m, idx) => (
                <div key={idx} className="relative">
                  <img src={m.url} className="max-h-48 rounded-lg object-cover w-full" />
                  <button
                    onClick={() => removeSelectedMedia(idx)}
                    className="absolute top-1 right-1 w-6 h-6 bg-black bg-opacity-50 text-white rounded-full text-xs"
                  >✕</button>
                </div>
              ))}
            </div>
          )}

          <div className="flex items-center justify-between ml-12">
            <button
              onClick={() => fileRef.current?.click()}
              className="text-sm text-gray-500 hover:text-blue-500 flex items-center gap-1"
            >
              📷 Thêm ảnh
            </button>
            <input ref={fileRef} type="file" accept="image/*" onChange={handleImage} className="hidden" multiple />
            <button
              onClick={handlePost}
              className="px-4 py-1.5 bg-blue-500 text-white text-sm rounded-full hover:bg-blue-600"
            >
              Đăng bài
            </button>
          </div>
        </div>

        {activeTag && (
          <div className="flex items-center gap-2 bg-blue-50 border border-blue-200 rounded-xl px-4 py-2">
            <span className="text-sm text-blue-600">Đang lọc: <strong>{activeTag}</strong></span>
            <button onClick={() => setActiveTag(null)} className="text-xs text-gray-400 hover:text-red-500 ml-auto">✕ Bỏ lọc</button>
          </div>
        )}

        {loading ? (
          <>
            <LoadingSkeleton />
            <LoadingSkeleton />
          </>
        ) : filteredPosts.length === 0 ? (
          <div className="bg-white border border-gray-200 rounded-xl p-8 text-center text-sm text-gray-400">
            Chưa có bài viết nào. Hãy đăng bài đầu tiên!
          </div>
        ) : (
          filteredPosts.map((post) => {
            const isOwnPost = user?.id === (post.author?.id ?? (post as any).authorId);
            return (
              <PostCard
                key={post.id}
                post={post}
                onDelete={isOwnPost ? () => handleDelete(post.id) : undefined}
              />
            );
          })
        )}
      </main>

      <aside className="hidden lg:flex w-60 flex-col gap-3">
        <div className="bg-white border border-gray-200 rounded-xl p-4 flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white font-medium">B</div>
          <div className="flex-1">
            <p className="text-sm font-medium text-gray-800">{user?.fullName ?? "error"}</p>
            <p className="text-xs text-gray-400">@{user?.userName ?? "error"}</p>
          </div>
          <button onClick={() => navigate("/profile")} className="text-xs text-blue-500 hover:underline">
            Xem
          </button>
        </div>
      </aside>
    </div>
  );
}

export default Home;