import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { authAPI } from "../api";
import PasswordStrength from "../components/ui/PasswordStrength";

interface RegisterForm {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

const Register = () => {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors } } = useForm<RegisterForm>();
  const passwordValue = watch("password", "");
  const [apiError, setApiError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const onSubmit = async (data: RegisterForm) => {
    setApiError(null);
    setIsSubmitting(true);

    try {
      await authAPI.register(data.username, data.username, data.email, data.password);
      navigate("/login");
    } catch (error: unknown) {
      const maybeMessage =
        typeof error === "object" &&
        error !== null &&
        "response" in error &&
        typeof (error as { response?: { data?: { errors?: string[]; message?: string } } }).response?.data?.message === "string"
          ? (error as { response?: { data?: { errors?: string[]; message?: string } } }).response?.data?.message
          : null;

      const maybeErrors =
        typeof error === "object" &&
        error !== null &&
        "response" in error
          ? (error as { response?: { data?: { errors?: string[] } } }).response?.data?.errors
          : undefined;

      setApiError(maybeErrors?.[0] ?? maybeMessage ?? "Đăng ký thất bại. Vui lòng thử lại.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center">
      <div className="bg-white rounded-xl border border-gray-200 p-8 w-full max-w-sm">
        <div className="flex items-center gap-2 mb-6">
          <div className="w-8 h-8 rounded-full bg-blue-500 flex items-center justify-center text-white text-sm font-medium">IH</div>
          <span className="font-medium text-gray-800">InteractHub</span>
        </div>

        <h2 className="text-xl font-medium text-gray-800 mb-6">Đăng ký</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
          <div>
            <label className="text-sm text-gray-600 mb-1 block">Tên người dùng</label>
            <input
              {...register("username", { required: "Vui lòng nhập tên người dùng" })}
              type="text"
              placeholder="nguyenvana"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.username && <p className="text-xs text-red-500 mt-1">{errors.username.message}</p>}
          </div>

          <div>
            <label className="text-sm text-gray-600 mb-1 block">Email</label>
            <input
              {...register("email", {
                required: "Vui lòng nhập email",
                pattern: { value: /^\S+@\S+$/i, message: "Email không hợp lệ" }
              })}
              type="email"
              placeholder="ban@example.com"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.email && <p className="text-xs text-red-500 mt-1">{errors.email.message}</p>}
          </div>

          <div>
            <label className="text-sm text-gray-600 mb-1 block">Mật khẩu</label>
            <input
              {...register("password", {
                required: "Vui lòng nhập mật khẩu",
                minLength: { value: 6, message: "Mật khẩu tối thiểu 6 ký tự" }
              })}
              type="password"
              placeholder="••••••••"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.password && <p className="text-xs text-red-500 mt-1">{errors.password.message}</p>}
            <PasswordStrength password={passwordValue} />
          </div>

          <div>
            <label className="text-sm text-gray-600 mb-1 block">Xác nhận mật khẩu</label>
            <input
              {...register("confirmPassword", {
                required: "Vui lòng xác nhận mật khẩu",
                validate: (val) => val === watch("password") || "Mật khẩu không khớp"
              })}
              type="password"
              placeholder="••••••••"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.confirmPassword && <p className="text-xs text-red-500 mt-1">{errors.confirmPassword.message}</p>}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600"
          >
            {isSubmitting ? "Đang đăng ký..." : "Đăng ký"}
          </button>
          {apiError && <p className="text-xs text-red-500">{apiError}</p>}
        </form>

        <p className="text-sm text-gray-500 text-center mt-4">
          Đã có tài khoản?{" "}
          <Link to="/login" className="text-blue-500">Đăng nhập</Link>
        </p>
      </div>
    </div>
  );
};

export default Register;