"use client";
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";

export default function RegisterPage() {
  const router = useNavigate();
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [retypepassword, setRetypepassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const res = await fetch("/api/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email,username, password, retypepassword }),
    });

    if (res.ok) {
      router("/login"); // chuyển hướng nếu register thành công
    } else {
      const data = await res.json();
      setError(data.message || "Đăng ký thất bại");
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50">
      <form onSubmit={handleSubmit} className="bg-white p-8 rounded-2xl shadow-lg w-96 space-y-4">
        <h1 className="text-2xl font-bold text-center">Đăng ký</h1>

        {error && <p className="text-red-500 text-center">{error}</p>}

        <input
          type="email"
          placeholder="Email"
          className="w-full border rounded-lg px-4 py-2"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <input
          type="username"
          placeholder="Tài khoản"
          className="w-full border rounded-lg px-4 py-2"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Mật khẩu"
          className="w-full border rounded-lg px-4 py-2"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Nhập lại mật khẩu"
          className="w-full border rounded-lg px-4 py-2"
          value={retypepassword}
          onChange={(e) => setRetypepassword(e.target.value)}
          required
        />

        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition border-0"
        >
          Đăng ký
        </button>
        <div className="text-center">
            <p>Bạn đã có tài khoản ?<Link to="/login" className="text-blue-500"> Đăng nhập</Link> </p>
        </div>
      </form>
    </div>
  );
}
