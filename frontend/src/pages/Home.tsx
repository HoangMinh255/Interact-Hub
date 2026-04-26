import { useState, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Sidebar from "../components/layout/Sidebar";
import PostCard from "../components/PostCard";
import LoadingSkeleton from "../components/ui/LoadingSkeleton";
import { usePosts } from "../hooks/usePosts";
import type { Post } from "../types";

const suggestions = [
  { id: "1", name: "Nguyễn Văn An", followers: 5 },
  { id: "2", name: "Trần Thị Bình", followers: 8 },
  { id: "3", name: "Lê Hoàng Nam", followers: 3 },
  { id: "4", name: "Phạm Thu Hà", followers: 6 },
  { id: "5", name: "Võ Minh Tuấn", followers: 4 },
];

const hashtags = [
  { tag: "#ReactJS", count: 1200 },
  { tag: "#LapTrinh", count: 980 },
  { tag: "#SaiGon", count: 754 },
  { tag: "#CongNghe", count: 632 },
];

function Home() {
  const { posts: initialPosts, loading } = usePosts();
  const [posts, setPosts] = useState<Post[]>([]);
  const [newPost, setNewPost] = useState("");
  const [previewImage, setPreviewImage] = useState<string | null>(null);
  const [followed, setFollowed] = useState<string[]>([]);
  const [activeTag, setActiveTag] = useState<string | null>(null);
  const fileRef = useRef<HTMLInputElement>(null);
  const navigate = useNavigate();

  if (initialPosts.length > 0 && posts.length === 0) {
    setPosts(initialPosts);
  }

  const handleImage = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => setPreviewImage(reader.result as string);
      reader.readAsDataURL(file);
    }
  };

  const handlePost = () => {
    if (!newPost.trim() && !previewImage) return;
    const post: Post = {
      id: Date.now().toString(),
      author: { id: "me", username: "Bạn", email: "me@gmail.com", followersCount: 0 },
      content: newPost,
      imageUrl: previewImage || undefined,
      likesCount: 0,
      commentsCount: 0,
      createdAt: "Vừa xong",
    };
    setPosts([post, ...posts]);
    setNewPost("");
    setPreviewImage(null);
    if (fileRef.current) fileRef.current.value = "";
  };

  const handleDelete = (id: string) => {
    setPosts(posts.filter((p) => p.id !== id));
  };

  const toggleFollow = (id: string) => {
    setFollowed((prev) =>
      prev.includes(id) ? prev.filter((f) => f !== id) : [...prev, id]
    );
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

          {previewImage && (
            <div className="relative mb-2 ml-12">
              <img src={previewImage} className="max-h-48 rounded-lg object-cover" />
              <button
                onClick={() => setPreviewImage(null)}
                className="absolute top-1 right-1 w-6 h-6 bg-black bg-opacity-50 text-white rounded-full text-xs"
              >✕</button>
            </div>
          )}

          <div className="flex items-center justify-between ml-12">
            <button
              onClick={() => fileRef.current?.click()}
              className="text-sm text-gray-500 hover:text-blue-500 flex items-center gap-1"
            >
              📷 Thêm ảnh
            </button>
            <input ref={fileRef} type="file" accept="image/*" onChange={handleImage} className="hidden" />
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
          filteredPosts.map((post) => (
            <PostCard
              key={post.id}
              post={post}
              onDelete={post.author.id === "me" ? () => handleDelete(post.id) : undefined}
            />
          ))
        )}
      </main>

      <aside className="hidden lg:flex w-60 flex-col gap-3">
        <div className="bg-white border border-gray-200 rounded-xl p-4 flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white font-medium">B</div>
          <div className="flex-1">
            <p className="text-sm font-medium text-gray-800">Bạn</p>
            <p className="text-xs text-gray-400">@nguoidung</p>
          </div>
          <button onClick={() => navigate("/profile")} className="text-xs text-blue-500 hover:underline">
            Xem
          </button>
        </div>

        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
          <p className="text-sm font-medium text-gray-800 px-4 pt-3 pb-2">Người bạn có thể biết</p>
          {suggestions.map((s) => (
            <div key={s.id} className="flex items-center gap-3 px-4 py-2 hover:bg-gray-50">
              <button onClick={() => navigate(`/profile/${s.id}`)} className="w-8 h-8 rounded-full bg-blue-400 flex items-center justify-center text-white text-xs font-medium">
                {s.name.charAt(0)}
              </button>
              <div className="flex-1">
                <button onClick={() => navigate(`/profile/${s.id}`)} className="text-xs font-medium text-gray-800 hover:text-blue-500 text-left">
                  {s.name}
                </button>
                <p className="text-xs text-gray-400">{s.followers} người theo dõi</p>
              </div>
              <button
                onClick={() => toggleFollow(s.id)}
                className={`text-xs px-3 py-1 rounded-full border ${
                  followed.includes(s.id)
                    ? "bg-blue-500 text-white border-blue-500"
                    : "border-blue-400 text-blue-400 hover:bg-blue-50"
                }`}
              >
                {followed.includes(s.id) ? "Đang theo dõi" : "Theo dõi"}
              </button>
            </div>
          ))}
        </div>

        <div className="bg-white border border-gray-200 rounded-xl p-4">
          <p className="text-sm font-medium text-gray-800 mb-2">Xu hướng cho bạn</p>
          {hashtags.map((h) => (
            <button
              key={h.tag}
              onClick={() => setActiveTag(activeTag === h.tag ? null : h.tag)}
              className={`w-full text-left py-1.5 rounded px-1 hover:bg-gray-50 ${activeTag === h.tag ? "bg-blue-50" : ""}`}
            >
              <p className="text-xs font-medium text-blue-500">{h.tag}</p>
              <p className="text-xs text-gray-400">{h.count.toLocaleString()} bài viết</p>
            </button>
          ))}
        </div>
      </aside>
    </div>
  );
}

export default Home;