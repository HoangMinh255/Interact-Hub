import { useState, useRef } from "react";
import { useForm } from "react-hook-form";

interface ProfileForm {
  username: string;
  bio: string;
}

function Profile() {
  const [isEditing, setIsEditing] = useState(false);
  const [username, setUsername] = useState("Người dùng");
  const [bio, setBio] = useState("Chưa có giới thiệu");
  const [avatar, setAvatar] = useState<string | null>(null);
  const [followers] = useState(0);
  const [following] = useState(0);
  const [postCount] = useState(0);
  const fileRef = useRef<HTMLInputElement>(null);

  const { register, handleSubmit, formState: { errors } } = useForm<ProfileForm>({
    defaultValues: { username, bio }
  });

  const onSubmit = (data: ProfileForm) => {
    setUsername(data.username);
    setBio(data.bio);
    setIsEditing(false);
  };

  const handleAvatar = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => setAvatar(reader.result as string);
      reader.readAsDataURL(file);
    }
  };

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <div className="bg-white border border-gray-200 rounded-xl overflow-hidden mb-4">
        <div className="h-36 bg-blue-100"></div>
        <div className="px-6 pb-6">
          <div className="flex items-end gap-4 -mt-10 mb-4">
            <div className="relative">
              {avatar ? (
                <img src={avatar} className="w-20 h-20 rounded-full object-cover border-4 border-white" />
              ) : (
                <div className="w-20 h-20 rounded-full bg-blue-500 flex items-center justify-center text-white text-2xl font-medium border-4 border-white">
                  {username.charAt(0).toUpperCase()}
                </div>
              )}
              <button
                onClick={() => fileRef.current?.click()}
                className="absolute bottom-0 right-0 w-6 h-6 bg-blue-500 rounded-full flex items-center justify-center text-white text-xs border-2 border-white"
              >
                📷
              </button>
              <input ref={fileRef} type="file" accept="image/*" onChange={handleAvatar} className="hidden" />
            </div>

            <div className="flex-1 pb-1">
              <h2 className="text-lg font-medium text-gray-800">{username}</h2>
              <p className="text-sm text-gray-400">@{username.toLowerCase().replace(/\s/g, "")}</p>
              <p className="text-sm text-gray-500 mt-1">{bio}</p>
            </div>

            <button
              onClick={() => setIsEditing(!isEditing)}
              className="px-4 py-2 bg-blue-500 text-white text-sm rounded-lg hover:bg-blue-600"
            >
              {isEditing ? "Hủy" : "Chỉnh sửa hồ sơ"}
            </button>
          </div>

          <div className="flex gap-6 text-sm text-gray-600">
            <span><strong className="text-gray-800">{postCount}</strong> bài viết</span>
            <span><strong className="text-gray-800">{followers}</strong> người theo dõi</span>
            <span><strong className="text-gray-800">{following}</strong> đang theo dõi</span>
          </div>
        </div>
      </div>

      {isEditing && (
        <div className="bg-white border border-gray-200 rounded-xl p-6 mb-4">
          <h3 className="text-sm font-medium text-gray-800 mb-4">Chỉnh sửa thông tin</h3>
          <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
            <div>
              <label className="text-sm text-gray-600 mb-1 block">Tên người dùng</label>
              <input
                {...register("username", { required: "Vui lòng nhập tên" })}
                className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
              />
              {errors.username && <p className="text-xs text-red-500 mt-1">{errors.username.message}</p>}
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
            <button type="submit" className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600">
              Lưu thay đổi
            </button>
          </form>
        </div>
      )}

      <div className="bg-white border border-gray-200 rounded-xl p-6">
        <p className="text-sm text-gray-400 text-center">Chưa có bài viết nào</p>
      </div>
    </div>
  );
}

export default Profile;