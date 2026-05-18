/* eslint-disable @typescript-eslint/no-explicit-any */
import { useEffect, useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Sidebar from "../components/layout/Sidebar";
import PostCard from "../components/PostCard";
import LoadingSkeleton from "../components/ui/LoadingSkeleton";
import { usePosts } from "../hooks/usePosts";
import { postsAPI, hashtagsApi } from "../api";
import type { Hashtag, Post } from "../types";
import { useAuth } from "../context/AuthContext";
import { IoCameraOutline } from "react-icons/io5";
import Avatar from "../components/ui/avatar";

function Home() {
  const { posts: initialPosts, loading } = usePosts();
  const [posts, setPosts] = useState<Post[]>([]);
  const [newPost, setNewPost] = useState("");
  const [visibility, setVisibility] = useState(0);
  const [hashtags, setHashtags] =useState<Hashtag[]>([]);
  const [activeTag, setActiveTag] = useState<string | null>(null);
  const fileRef = useRef<HTMLInputElement>(null);
  const navigate = useNavigate();
  const [selectedMedia, setSelectedMedia] = useState<Array<{url: string, type: number, file: File}>>([]);
  const { user } = useAuth();


  useEffect(() => {
    if (initialPosts.length > 0 && posts.length === 0) {
      setPosts(initialPosts);
    }
    const loadHashtags = async () => {
      try {
        const response = await hashtagsApi.get5TrendingHashtags();
        
        // Bóc tách dữ liệu qua các lớp của Backend trả về
        const hashtagArray = response.data?.data?.trendingHashtags || response.data?.trendingHashtags || [];
        
        // Đảm bảo chỉ set vào state nếu nó thực sự là một mảng
        if (Array.isArray(hashtagArray)) {
          setHashtags(hashtagArray);
        } else {
          setHashtags([]);
        }

      } catch (error : any){
        const errorMsg = error.response?.data?.message || error.message || "Lấy dữ liệu hashtag thất bại!";
        console.error("Hashtag load error:", errorMsg);
      }
    }
    loadHashtags();
    
  }, [initialPosts, posts.length]);
  const handleImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      const maxFiles = 4;
      const arr = Array.from(files).slice(0, maxFiles - selectedMedia.length);
      arr.forEach((file) => {
        const previewUrl = URL.createObjectURL(file);
        const isVideo = file.type.startsWith('video/') ? 1 : 0;
        
        setSelectedMedia((prev) => [...prev, { url: previewUrl, type: isVideo, file: file }]);
      });
    }
    if (e.target) e.target.value = "";
  };

  const removeSelectedMedia = (index: number) => {
    setSelectedMedia((prev) => prev.filter((_, i) => i !== index));
  };

  const handlePost = async () => {
    if (!newPost.trim() && selectedMedia.length === 0) return;

    // 1. Khởi tạo FormData
    const formData = new FormData();
    formData.append("Content", newPost.trim());
    formData.append("Visibility", visibility.toString());

    // 2. Đính kèm các file thực tế vào formData
    selectedMedia.forEach((m) => {
      formData.append("files", m.file); 
      console.log(`Đã thêm file vào FormData: ${m.file.name} (${m.file.type})`);
    });

    try {
      // 3. Truyền thẳng formData vào API
      const response = await postsAPI.create(formData);
      console.log("Post created:", response.data);

      setNewPost("");
      setVisibility(0);
      
      // Xóa preview URLs khỏi bộ nhớ của trình duyệt để tránh tràn RAM
      selectedMedia.forEach(m => URL.revokeObjectURL(m.url));
      setSelectedMedia([]);

      // 4. Reload lại posts để lấy dữ liệu mới
      const freshResponse = await postsAPI.getAll();
      console.log("Raw API response:", freshResponse.data);
      const mappedPosts = Array.isArray(freshResponse.data)
        ? freshResponse.data.map((p: any) => {
            if (p.isShared) {
              console.log(`Shared post ${p.id}: content="${p.content}" vs originalContent="${p.originalContent}"`);
            }
            return {
            id: p.id,
            visibility: p.visibility,
            authorName: p.authorName ?? "Unknown",
            authorAvatar: p.authorAvatar ?? "",
            authorId: p.authorId,
            content: p.content,
            mediaUrls: p.mediaUrls ?? [],
            likesCount: 0,
            commentCount: p.commentCount ?? 0,
            createdAt: new Date(p.createdAt).toLocaleString("vi-VN"),
            isShared: p.isShared ?? false,
            originalContent: p.originalContent ?? "",
            originalAuthorId: p.originalAuthorId ?? null,
            originalAuthorName: p.originalAuthorName ?? "",
            originalAuthorAvatar: p.originalAuthorAvatar ?? undefined,
            originalPostId: p.originalPostId,
            hashtag: p.hashtags || p.hashtag || [],
            };
          })
        : [];
      setPosts(mappedPosts);
    } catch (error: any) {
      const errorMsg = error.response?.data?.message || error.message || "Đăng bài thất bại";
      console.error("Post creation error:", errorMsg);
      alert(`❌ Lỗi: ${errorMsg}`);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await postsAPI.delete(id);
      setPosts(posts.filter((p) => p.id !== id));
      alert("Bài viết đã được xóa");
    } catch (error: any) {
      const errorMsg = error.response?.data?.message || error.message || "Xóa bài viết thất bại";
      console.error("Post deletion error:", errorMsg);
      alert(`Lỗi: ${errorMsg}`);
    }
  };

  const filteredPosts = activeTag
    ? posts.filter((p: any) => {
        // kiểm tra chính xác trong mảng hashtag
        if (p.hashtag && p.hashtag.length > 0) {
          const cleanActiveTag = activeTag.replace('#', '').toLowerCase();
          const hasMatch = p.hashtag.some((t: string) => 
            t.replace('#', '').toLowerCase() === cleanActiveTag
          );
          if (hasMatch) return true;
        }
        
        // Tìm bằng Regex trong nội dung
        const regex = new RegExp(`#${activeTag.replace('#', '')}(?![A-Za-z0-9_])`, 'i');
        return regex.test(p.content);
      })
    : posts;

  return (
    <div className="max-w-5xl mx-auto px-4 py-4 flex gap-4">
      <div className="hidden md:block">
        <Sidebar />
      </div>
      <main className="flex-1 flex flex-col gap-3">
        <div className="bg-white border border-gray-200 rounded-xl p-3">
          <div className="flex items-center gap-3 mb-2">
            <Avatar name={user?.fullName ?? "B"} avatarUrl={user?.avatarUrl ?? null} size="md" />
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
              <IoCameraOutline /> Thêm ảnh
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
            const isAuthor = user?.id === (post.authorId ?? (post as any).authorId ?? null);
            return (
              <PostCard
                key={post.id}
                post={post}
                onDelete={isAuthor ? () => handleDelete(post.id) : undefined}
                onUpdate={isAuthor ? (updatedContent) => {
                  setPosts(posts.map(p => p.id === post.id ? { ...p, content: updatedContent } : p));
                } : undefined}
                onHashtagClick={(tag) => {setActiveTag(tag);
                                          window.scrollTo({ top: 0, behavior: 'smooth' });
                }}  
              />
            );
          })
        )}
      </main>

      <aside className="hidden lg:flex w-60 flex-col gap-3">
        <div className="bg-white border border-gray-200 rounded-xl p-4 flex items-center gap-3">
          <Avatar name={user?.fullName ?? "B"} avatarUrl={user?.avatarUrl ?? null} size="md" />
          <div className="flex-1">
            <p className="text-sm font-medium text-gray-800">{user?.fullName ?? "error"}</p>
            <p className="text-xs text-gray-400">@{user?.userName ?? "error"}</p>
          </div>
          <button onClick={() => navigate("/profile")} className="text-xs text-blue-500 hover:underline">
            Xem
          </button>
        </div>
        <div className="bg-white border border-gray-200 rounded-xl p-4">
          <p className="text-sm font-medium text-gray-800 mb-2">Xu hướng cho bạn</p>
          {hashtags && hashtags.length > 0 ? (
            hashtags.map((h: any, index: number) => {
              // Dùng || để chấp nhận cả chữ hoa lẫn chữ thường
              const tagName = h.tag || h.Tag || "Unknown";
              const postCount = h.count || h.Count || 0;

              return (
                <button
                  key={index}
                  onClick={() => setActiveTag(activeTag === tagName ? null : tagName)}
                  className={`w-full text-left py-1.5 rounded px-1 hover:bg-gray-50 transition-colors ${
                    activeTag === tagName ? "bg-blue-50 text-blue-700" : ""
                  }`}
                >
                  <p className="text-xs font-medium text-blue-500">#{tagName}</p>
                  <p className="text-xs text-gray-400">
                    {postCount.toLocaleString()} bài viết
                  </p>
                </button>
              );
            })
          ) : (
            <div className="py-4 text-center">
              <div className="animate-spin inline-block w-4 h-4 border-2 border-blue-500 border-t-transparent rounded-full mb-2"></div>
              <p className="text-[10px] text-gray-400">Đang tìm xu hướng...</p>
            </div>
          )}
        </div>
      </aside>
    </div>
  );
}

export default Home;