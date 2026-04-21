import { useForm } from "react-hook-form";
import { Link } from "react-router-dom";

interface LoginForm {
  email: string;
  password: string;
}

function Login() {
  const { register, handleSubmit, formState: { errors } } = useForm<LoginForm>();

  const onSubmit = (data: LoginForm) => {
    console.log(data);
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
                required: "Vui lòng nhập mật khẩu",
                minLength: { value: 6, message: "Mật khẩu tối thiểu 6 ký tự" }
              })}
              type="password"
              placeholder="••••••••"
              className="w-full h-10 border border-gray-200 rounded-lg px-3 text-sm outline-none focus:border-blue-400"
            />
            {errors.password && <p className="text-xs text-red-500 mt-1">{errors.password.message}</p>}
          </div>

          <button
            type="submit"
            className="w-full h-10 bg-blue-500 text-white rounded-lg text-sm font-medium hover:bg-blue-600"
          >
            Đăng nhập
          </button>
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