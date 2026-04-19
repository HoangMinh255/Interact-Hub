import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface EmailForm {
  email: string;
}

interface PasswordForm {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

function Settings() {
  const { logout } = useAuth();
  const navigate = useNavigate();
  const [tab, setTab] = useState<"email" | "password" | "danger">("email");
  const [emailSuccess, setEmailSuccess] = useState(false);
  const [passwordSuccess, setPasswordSuccess] = useState(false);

  const emailForm = useForm<EmailForm>();
  const passwordForm = useForm<PasswordForm>();

  const onEmailSubmit = (data: EmailForm) => {
    console.log("Đổi email:", data);
    setEmailSuccess(true);
    setTimeout(() => setEmailSuccess(false), 3000);
  };

  const onPasswordSubmit = (data: PasswordForm) => {
    console.log("Đổi mật khẩu:", data);
    setPasswordSuccess(true);
    setTimeout(() => setPasswordSuccess(false), 3000);
  };

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="max-w-2xl mx-auto px-4 py-6 flex flex-col gap-4">
      <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
        <h2 className="text-base font-medium text-gray-800 px-6 py-4 border-b border-gray-100">
          ⚙️ Cài đặt tài khoản
        </h2>

        {/* Tab */}
        <div className="flex border-b border-gray-100">
          {[
            { key: "email", label: "📧 Đổi email" },
            { key: "password", label: "🔒 Đổi mật khẩu" },
            { key: "danger", label: "⚠️ Tài khoản" },
          ].map((t) => (
            <button
              key={t.key}
              onClick={() => setTab(t.key as "email" | "password" | "danger")}
              className={`flex-1 py-3 text-sm ${
                tab === t.key
                  ? "text-blue-500 border-b-2 border-blue-500 font-medium"
                  : "text-gray-500 hover:text-gray-700"
              }`}
            >
              {t.label}
            </button>
          ))}
        </div>

        <div className="p-6">
          {/* Đổi email */}
          {tab === "email" && (
            <form onSubmit={emailForm.handleSubmit(onEmailSubmit)} className="flex flex-col gap-4">
              <div>
                <label className="text-sm text-gray-600 mb-1 block">Email hiện tại</label>
                <input
                  type="email"
                  disabled
                  value="ban@example.com"
                  className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm bg-gray-50 text-gray-400"
                />
              </div>
              <div>
                <label className="text-sm text-gray-600 mb-1 block">Email mới</label>
                <input
                  {...emailForm.register("email", {
                    required: "Vui lòng nhập email mới",
                    pattern: { value: /^\S+@\S+$/i, message: "Email không hợp lệ" },
                  })}
                  type="email"
                  placeholder="email_moi@example.com"
                  className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
                />
                {emailForm.formState.errors.email && (
                  <p className="text-xs text-red-500 mt-1">{emailForm.formState.errors.email.message}</p>
                )}
              </div>
              {emailSuccess && <p className="text-xs text-green-500">✅ Đổi email thành công!</p>}
              <button type="submit" className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600">
                Cập nhật email
              </button>
            </form>
          )}

          {/* Đổi mật khẩu */}
          {tab === "password" && (
            <form onSubmit={passwordForm.handleSubmit(onPasswordSubmit)} className="flex flex-col gap-4">
              <div>
                <label className="text-sm text-gray-600 mb-1 block">Mật khẩu hiện tại</label>
                <input
                  {...passwordForm.register("currentPassword", { required: "Vui lòng nhập mật khẩu hiện tại" })}
                  type="password"
                  placeholder="••••••••"
                  className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
                />
                {passwordForm.formState.errors.currentPassword && (
                  <p className="text-xs text-red-500 mt-1">{passwordForm.formState.errors.currentPassword.message}</p>
                )}
              </div>
              <div>
                <label className="text-sm text-gray-600 mb-1 block">Mật khẩu mới</label>
                <input
                  {...passwordForm.register("newPassword", {
                    required: "Vui lòng nhập mật khẩu mới",
                    minLength: { value: 6, message: "Mật khẩu tối thiểu 6 ký tự" },
                  })}
                  type="password"
                  placeholder="••••••••"
                  className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
                />
                {passwordForm.formState.errors.newPassword && (
                  <p className="text-xs text-red-500 mt-1">{passwordForm.formState.errors.newPassword.message}</p>
                )}
              </div>
              <div>
                <label className="text-sm text-gray-600 mb-1 block">Xác nhận mật khẩu mới</label>
                <input
                  {...passwordForm.register("confirmPassword", {
                    required: "Vui lòng xác nhận mật khẩu",
                    validate: (val) =>
                      val === passwordForm.watch("newPassword") || "Mật khẩu không khớp",
                  })}
                  type="password"
                  placeholder="••••••••"
                  className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
                />
                {passwordForm.formState.errors.confirmPassword && (
                  <p className="text-xs text-red-500 mt-1">{passwordForm.formState.errors.confirmPassword.message}</p>
                )}
              </div>
              {passwordSuccess && <p className="text-xs text-green-500">✅ Đổi mật khẩu thành công!</p>}
              <button type="submit" className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600">
                Cập nhật mật khẩu
              </button>
            </form>
          )}

          {/* Tài khoản */}
          {tab === "danger" && (
            <div className="flex flex-col gap-4">
              <div className="border border-gray-200 rounded-lg p-4 flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-gray-800">Đăng xuất</p>
                  <p className="text-xs text-gray-400 mt-0.5">Thoát khỏi tài khoản hiện tại</p>
                </div>
                <button
                  onClick={handleLogout}
                  className="px-4 py-2 bg-gray-100 text-gray-600 text-sm rounded-lg hover:bg-gray-200"
                >
                  Đăng xuất
                </button>
              </div>

              <div className="border border-red-200 rounded-lg p-4 flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-red-600">Xóa tài khoản</p>
                  <p className="text-xs text-gray-400 mt-0.5">Hành động này không thể hoàn tác</p>
                </div>
                <button
                  onClick={() => confirm("Bạn có chắc muốn xóa tài khoản?") && alert("Tài khoản đã bị xóa!")}
                  className="px-4 py-2 bg-red-500 text-white text-sm rounded-lg hover:bg-red-600"
                >
                  Xóa tài khoản
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}

export default Settings;