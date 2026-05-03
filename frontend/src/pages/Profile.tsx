import { useState, useRef, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { useAuth } from "../context/AuthContext";
import PostCard from "../components/PostCard";
import { storiesAPI, postsAPI, usersAPI, notificationsAPI } from "../api";
import type { Post } from "../types";

interface ProfileForm {
  fullName: string;
  bio: string;
}

interface StoryItem {
  id: string;
  content?: string | null;
  mediaUrl?: string | null;
  mediaType?: string | null;
  expireAt: string;
  isActive: boolean;
  createdAt: string;
}

function Profile() {
  const [isEditing, setIsEditing] = useState(false);
  const { user: authUser } = useAuth();
  const { userId } = useParams();
  const [viewedUser, setViewedUser] = useState(authUser);
  const [bio, setBio] = useState("");
  const [fullName, setFullName] = useState("");
  const [avatar, setAvatar] = useState<string | null>(null);
  const [followers] = useState(0);
  const [following] = useState(0);
  const [posts, setPosts] = useState<Post[]>([]);
  const [postsLoading, setPostsLoading] = useState(false);
  const [postPage, setPostPage] = useState(0);
  const [hasMorePosts, setHasMorePosts] = useState(true);
  const [stories, setStories] = useState<StoryItem[]>([]);
  const [storiesLoading, setStoriesLoading] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [friendRequestSent, setFriendRequestSent] = useState(false);
  const [friendLoading, setFriendLoading] = useState(false);
  const profileUserId = userId ?? authUser?.id;
  const isOwnProfile = !userId || userId === authUser?.id;
  const fileRef = useRef<HTMLInputElement>(null);

  const { register, handleSubmit, formState: { errors }, setValue } = useForm<ProfileForm>({
    defaultValues: { fullName: viewedUser?.fullName, bio: viewedUser?.bio || "Chưa có giới thiệu" }
  });

  useEffect(() => {
    setPosts([]);
    setPostPage(0);
    setHasMorePosts(true);
  }, [profileUserId]);

  useEffect(() => {
    const loadProfile = async () => {
      try {
        if (userId && authUser?.id !== userId) {
          const response = await usersAPI.getUserById(userId);
          const u = response.data?.data;
          if (u) {
            setViewedUser(u);
            setBio(u.bio || "Chưa có giới thiệu");
            setFullName(u.fullName);
            setAvatar(u.avatarUrl || null);
            setValue("fullName", u.fullName);
            setValue("bio", u.bio || "Chưa có giới thiệu");
          }
          return;
        }

        if (authUser) {
          setViewedUser(authUser);
          setBio(authUser.bio || "Chưa có giới thiệu");
          setFullName(authUser.fullName);
          setAvatar(authUser.avatarUrl || null);
          setValue("fullName", authUser.fullName);
          setValue("bio", authUser.bio || "Chưa có giới thiệu");
        }
      } catch (error) {
        console.error("Failed to load profile:", error);
      }
    };

    const loadStories = async () => {
      try {
        if (!isOwnProfile) {
          setStories([]);
          return;
        }

        setStoriesLoading(true);
        const response = await storiesAPI.getMyStories();
        const items = response.data?.data ?? [];
        setStories(items);
      } catch (error) {
        console.error("Failed to load stories:", error);
        setStories([]);
      } finally {
        setStoriesLoading(false);
      }
    };

    loadProfile();
    loadStories();
  }, [authUser, userId, isOwnProfile, setValue]);

  useEffect(() => {
    const loadPosts = async () => {
      if (!profileUserId) {
        setPosts([]);
        setHasMorePosts(false);
        return;
      }

      try {
        setPostsLoading(true);
        const response = await postsAPI.getByUser(profileUserId, postPage);
        const items = Array.isArray(response.data) ? response.data : [];
        const mappedPosts: Post[] = items.map((item: any) => ({
          id: item.id,
          content: item.content ?? "",
          visibility: item.visibility ?? 0,
          createdAt: item.createdAt ?? "",
          authorName: item.authorName ?? viewedUser?.fullName ?? "Unknown",
          authorAvatar: item.authorAvatar ?? viewedUser?.avatarUrl,
          author: viewedUser?.id ? { id: viewedUser.id, fullName: viewedUser.fullName, userName: viewedUser.userName ?? "", email: viewedUser.email ?? "", followersCount: viewedUser.followersCount ?? 0 } : undefined,
          mediaUrls: item.mediaUrls ?? [],
          likesCount: item.likesCount ?? 0,
          commentCount: item.commentCount ?? 0,
        }));

        setPosts((prev) => (postPage === 0 ? mappedPosts : [...prev, ...mappedPosts]));
        setHasMorePosts(mappedPosts.length === 10);
      } catch (error) {
        console.error("Failed to load posts:", error);
        if (postPage === 0) {
          setPosts([]);
        }
        setHasMorePosts(false);
      } finally {
        setPostsLoading(false);
      }
    };

    void loadPosts();
  }, [profileUserId, postPage, viewedUser?.fullName, viewedUser?.avatarUrl]);

  useEffect(() => {
    if (!isOwnProfile) {
      setIsEditing(false);
    }
  }, [isOwnProfile]);

  const onSubmit = async (data: ProfileForm) => {
    try {
      setIsLoading(true);
      await usersAPI.updateProfile(data.fullName, data.bio);
      setFullName(data.fullName);
      setBio(data.bio);
      setViewedUser((prev) => prev ? { ...prev, fullName: data.fullName, bio: data.bio } : prev);
      setIsEditing(false);
    } catch (error) {
      console.error("Failed to update profile:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleAvatar = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      try {
        setIsLoading(true);

        const previewUrl = URL.createObjectURL(file);
        setAvatar(previewUrl);

        const response = await usersAPI.uploadAvatar(file);
        const uploadedAvatarUrl = response.data?.data?.avatarUrl;

        if (uploadedAvatarUrl) {
          setAvatar(uploadedAvatarUrl);
          setViewedUser((prev) => prev ? { ...prev, avatarUrl: uploadedAvatarUrl } : prev);
        }
      } catch (error) {
        console.error("Failed to upload avatar:", error);
      } finally {
        setIsLoading(false);
        e.target.value = "";
      }
    }
  };

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <div className="bg-white border border-gray-200 rounded-xl overflow-hidden mb-4">
        <div className="h-36 bg-blue-100"></div>
        <div className="px-6 pb-6">
          <div className="flex flex-col sm:flex-row items-start sm:items-end gap-4 -mt-10 mb-4">
            <div className="relative">
              {avatar ? (
                <img src={avatar} className="w-20 h-20 rounded-full object-cover border-4 border-white" />
              ) : (
                <div className="w-20 h-20 rounded-full bg-blue-500 flex items-center justify-center text-white text-2xl font-medium border-4 border-white">
                  {viewedUser?.userName.charAt(0).toUpperCase()}
                </div>
              )}
              {isOwnProfile && (
                <>
                  <button
                    onClick={() => fileRef.current?.click()}
                    className="absolute bottom-0 right-0 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center text-white text-xs border-2 border-white"
                  >
                    📷
                  </button>
                  <input ref={fileRef} type="file" accept="image/*" onChange={handleAvatar} className="hidden" />
                </>
              )}
            </div>

            <div className="flex-1 pb-1">
              <h2 className="text-lg font-medium text-gray-800">{fullName}</h2>
              <p className="text-sm text-gray-400">@{viewedUser?.userName.toLowerCase().replace(/\s/g, "")}</p>
              <p className="text-sm text-gray-500 mt-1">{bio}</p>
            </div>

              {!isOwnProfile && authUser && (
                <div className="flex items-center gap-2">
                  <button
                    onClick={async () => {
                      if (!authUser?.id || !profileUserId) return;
                      try {
                        setFriendLoading(true);
                        await (await import("../api")).friendsAPI.sendRequest(authUser.id, profileUserId);
                        
                        // Tạo thông báo gửi cho người kia
                        const notificationData = {
                          recipientId: profileUserId, // Người nhận là chủ của Profile này
                          actorId: authUser.id,       // Người thực hiện (người đang đăng nhập)
                          type: 2,                    // Quy ước Type 2 là Lời mời kết bạn
                          content: `${authUser.fullName} đã gửi cho bạn một lời mời kết bạn.`,
                          relatedEntityType: "User",  
                          relatedEntityId: authUser.id,
                          createdAt: new Date().toISOString()
                        };

                        await (await import("../api")).notificationsAPI.create(notificationData);

                        setFriendRequestSent(true);
                        
                      } catch (err) {
                        console.error("Failed to send friend request", err);
                      } finally {
                        setFriendLoading(false);
                      }
                    }}
                    disabled={friendLoading || friendRequestSent}
                    className={"px-4 py-2 text-sm rounded-lg " + (friendRequestSent ? "bg-gray-200 text-gray-600" : "bg-blue-500 text-white hover:bg-blue-600")}
                  >
                    {friendLoading ? "Đang gửi..." : friendRequestSent ? "Đã gửi" : "Kết bạn"}
                  </button>
                </div>
              )}

              {isOwnProfile && (
              <button
                onClick={() => setIsEditing(!isEditing)}
                className="w-full sm:w-auto px-4 py-2 bg-blue-500 text-white text-sm rounded-lg hover:bg-blue-600"
              >
                {isEditing ? "Hủy" : "Chỉnh sửa hồ sơ"}
              </button>
            )}
          </div>

          <div className="flex gap-6 text-sm text-gray-600">
            <span><strong className="text-gray-800">{posts.length}</strong> bài viết</span>
            <span><strong className="text-gray-800">{followers}</strong> người theo dõi</span>
            <span><strong className="text-gray-800">{following}</strong> đang theo dõi</span>
          </div>
        </div>
      </div>

      {isOwnProfile && (
        <div className="bg-white border border-gray-200 rounded-xl p-6 mb-4">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-sm font-medium text-gray-800">Story của bạn</h3>
            <span className="text-xs text-gray-400">{stories.length} story</span>
          </div>

          {storiesLoading ? (
            <p className="text-sm text-gray-400">Đang tải story...</p>
          ) : stories.length === 0 ? (
            <p className="text-sm text-gray-400">Bạn chưa đăng story nào.</p>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
              {stories.map((story) => (
                <div key={story.id} className="border border-gray-200 rounded-xl p-3">
                  {story.mediaUrl && (
                    <div className="mb-2 overflow-hidden rounded-lg bg-gray-100">
                      {story.mediaType?.startsWith("video") ? (
                        <video src={story.mediaUrl} controls className="w-full max-h-56 object-cover" />
                      ) : (
                        <img src={story.mediaUrl} alt="Story media" className="w-full max-h-56 object-cover" />
                      )}
                    </div>
                  )}
                  {story.content && <p className="text-sm text-gray-700 mb-2">{story.content}</p>}
                  <div className="flex items-center justify-between text-xs text-gray-400">
                    <span>{new Date(story.createdAt).toLocaleString("vi-VN")}</span>
                    <span>{story.isActive ? "Đang hoạt động" : "Đã ẩn"}</span>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {isOwnProfile && isEditing && (
        <div className="bg-white border border-gray-200 rounded-xl p-6 mb-4">
          <h3 className="text-sm font-medium text-gray-800 mb-4">Chỉnh sửa thông tin</h3>
          <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
            <div>
              <label className="text-sm text-gray-600 mb-1 block">Tên đầy đủ</label>
              <input
                {...register("fullName", { required: "Vui lòng nhập tên đầy đủ" })}
                className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
              />
              {errors.fullName && <p className="text-xs text-red-500 mt-1">{errors.fullName.message}</p>}
            </div>
            <div>
              <label className="text-sm text-gray-600 mb-1 block">Giới thiệu bản thân</label>
              <textarea
                {...register("bio")}
                rows={3}
                className="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm outline-none focus:border-blue-400 resize-none"
                placeholder="Viết gì đó về bản thân..."
              />
            </div>
            <button type="submit" disabled={isLoading} className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed">
              {isLoading ? "Đang lưu..." : "Lưu thay đổi"}
            </button>
          </form>
        </div>
      )}

      <div className="bg-white border border-gray-200 rounded-xl p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-sm font-medium text-gray-800">Bài viết</h3>
          <span className="text-xs text-gray-400">{posts.length} bài viết</span>
        </div>

        {postsLoading && postPage === 0 ? (
          <p className="text-sm text-gray-400 text-center">Đang tải bài viết...</p>
        ) : posts.length === 0 ? (
          <p className="text-sm text-gray-400 text-center">Chưa có bài viết nào</p>
        ) : (
          <div className="flex flex-col gap-4">
            {posts.map((post) => (
              <PostCard key={post.id} post={post} />
            ))}

            {hasMorePosts && (
              <button
                type="button"
                onClick={() => setPostPage((current) => current + 1)}
                disabled={postsLoading}
                className="self-center px-4 py-2 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 disabled:opacity-50"
              >
                {postsLoading ? "Đang tải..." : "Tải thêm"}
              </button>
            )}
          </div>
        )}
      </div>
    </div>
  );
}

export default Profile;
