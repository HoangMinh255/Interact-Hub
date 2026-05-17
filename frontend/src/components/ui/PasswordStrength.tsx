interface PasswordStrengthProps {
  password: string;
}

interface StrengthResult {
  score: number;       // 0–4
  label: string;
  color: string;
  checks: {
    label: string;
    passed: boolean;
  }[];
}

function getStrength(password: string): StrengthResult {
  const checks = [
    { label: "Ít nhất 8 ký tự", passed: password.length >= 8 },
    { label: "Chữ hoa (A-Z)", passed: /[A-Z]/.test(password) },
    { label: "Chữ thường (a-z)", passed: /[a-z]/.test(password) },
    { label: "Số (0-9)", passed: /[0-9]/.test(password) },
    { label: "Ký tự đặc biệt (!@#$...)", passed: /[^A-Za-z0-9]/.test(password) },
  ];

  const score = checks.filter((c) => c.passed).length;

  const levels = [
    { label: "null", color: "bg-gray-200"},
    { label: "Rất yếu", color: "bg-red-500" },
    { label: "Yếu", color: "bg-orange-400" },
    { label: "Trung bình", color: "bg-yellow-400" },
    { label: "Mạnh", color: "bg-blue-500" },
    { label: "Rất mạnh", color: "bg-green-500" },
  ];

  return {
    score,
    label: levels[score].label,
    color: levels[score].color,
    checks,
  };
}

const PasswordStrength = ({ password }: PasswordStrengthProps) => {
  if (!password) return null;

  const { score, label, color, checks } = getStrength(password);

  return (
    <div className="mt-2 flex flex-col gap-2">

      {/* Strength bars */}
      <div className="flex gap-1">
        {[1, 2, 3, 4, 5].map((i) => (
          <div
            key={i}
            className={`h-1.5 flex-1 rounded-full transition-all duration-300 ${
              i <= score ? color : "bg-gray-200"
            }`}
          />
        ))}
      </div>

      {/* Label */}
      <p className={`text-xs font-medium ${
        score <= 1 ? "text-red-500" :
        score === 2 ? "text-yellow-500" :
        score === 3 ? "text-blue-500" :
        "text-green-500"
      }`}>
        {label}
      </p>

      {/* Checklist */}
      <ul className="flex flex-col gap-1">
        {checks.map((check) => (
          <li key={check.label} className="flex items-center gap-2 text-xs">
            <span className={check.passed ? "text-green-500" : "text-gray-300"}>
              {check.passed ? "✓" : "○"}
            </span>
            <span className={check.passed ? "text-gray-600" : "text-gray-400"}>
              {check.label}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default PasswordStrength;