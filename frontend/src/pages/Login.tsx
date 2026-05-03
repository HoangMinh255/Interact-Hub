import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { authAPI } from "../api";
import { useAuth } from "../context/AuthContext";

interface LoginForm {
  email: string;
  password: string;
}

function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const { register, handleSubmit, formState: { errors } } = useForm<LoginForm>();
  const [apiError, setApiError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const onSubmit = async (data: LoginForm) => {
    setApiError(null);
    setIsSubmitting(true);

    try {
      const response = await authAPI.login(data.email, data.password);
      const payload = response.data?.data;

      if (!payload?.accessToken || !payload?.user) {
        setApiError("Phản hồi đăng nhập không hợp lệ từ máy chủ.");
        return;
      }

      login(payload.accessToken, {
        id: payload.user.id,
        userName: payload.user.userName,
        email: payload.user.email,
        followersCount: 0,
        fullName: payload.user.fullName,
      });

      navigate("/");
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

      setApiError(maybeErrors?.[0] ?? maybeMessage ?? "Đăng nhập thất bại. Vui lòng thử lại.");
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

        <h2 className="text-xl font-medium text-gray-800 mb-6">Đăng nhập</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-4">
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
                required: "Vui lòng nhập mật khẩu"
              })}
              type="password"
              placeholder="••••••••"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.password && <p className="text-xs text-red-500 mt-1">{errors.password.message}</p>}
          </div>

          <button
            type="submit"
            disabled={isSubmitting}
            className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600"
          >
            {isSubmitting ? "Đang đăng nhập..." : "Đăng nhập"}
          </button>
          {apiError && <p className="text-xs text-red-500">{apiError}</p>}
        </form>

        <p className="text-sm text-gray-500 text-center mt-4">
          Chưa có tài khoản?{" "}
          <Link to="/register" className="text-blue-500">Đăng ký ngay</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;