import { useState } from "react";
import { useNavigate, Link} from "react-router-dom";
import ClipLoader from "react-spinners/ClipLoader";
export default function LoginPage() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [formErrors, setFormErrors] = useState<Record<string, string>>({});
  const [isLoading, setIsLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      setIsLoading(true);

      const res = {
        username: "admin",
        password: "123456",
        role: "Admin",
      };

      localStorage.setItem("user", JSON.stringify(res));
      if(username === "admin" && password === "123456")
      {
        console.log("Login success, moving to dashboard...");
        navigate("/dashboard");
      }
    } catch (err) {
      console.log(err);
      setFormErrors({
        username: "Đăng nhập thất bại",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50">
      {isLoading && (
        <div className="fixed inset-0 bg-black/30 flex justify-center items-center z-50">
          <ClipLoader color="#ffffff" size={80} />
        </div>
      )}

      <form
        onSubmit={handleSubmit}
        className="bg-white p-8 rounded-2xl shadow-lg w-96 space-y-4"
      >
        <h1 className="text-2xl font-bold text-center">Đăng nhập</h1>

        <input
          type="text"
          placeholder="Tài khoản"
          className="w-full border rounded-lg px-4 py-2"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />
        {formErrors.username && (
          <p className="text-red-500 text-sm">{formErrors.username}</p>
        )}

        <input
          type="password"
          placeholder="Mật khẩu"
          className="w-full border rounded-lg px-4 py-2"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        {formErrors.password && (
          <p className="text-red-500 text-sm">{formErrors.password}</p>
        )}

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition border-0"
        >
          Đăng nhập
        </button>

        <div className="text-center">
          <p>
            Bạn chưa có tài khoản?
            <Link to="/register" className="text-blue-500">
              {" "}Đăng ký
            </Link>
          </p>
        </div>
      </form>
    </div>
  );
}