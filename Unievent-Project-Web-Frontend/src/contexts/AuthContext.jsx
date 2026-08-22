import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { isUserSecretary } from "../services/authService.js";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

  // Restaurar sessão ao montar componente
  useEffect(() => {
    const storedToken = localStorage.getItem("authToken");
    const storedUser = localStorage.getItem("authUser");

    if (storedToken && storedUser) {
      try {
        const parsedUser = JSON.parse(storedUser);

        // Validar que é um UsuarioSecretaria
        if (!isUserSecretary(parsedUser)) {
          localStorage.removeItem("authToken");
          localStorage.removeItem("authUser");
          setError(
            "Acesso negado: apenas usuários administrativos podem usar este sistema",
          );
          setIsLoading(false);
          return;
        }

        setToken(storedToken);
        setUser(parsedUser);
      } catch (err) {
        localStorage.removeItem("authToken");
        localStorage.removeItem("authUser");
        setError("Erro ao restaurar sessão");
      }
    }

    setIsLoading(false);
  }, []);

  // Fazer login
  const login = (userData, authToken) => {
    if (!isUserSecretary(userData)) {
      setError("Acesso negado: apenas usuários administrativos podem usar este sistema");
      return false;
    }

    setUser(userData);
    setToken(authToken);

    localStorage.setItem("authToken", authToken);
    localStorage.setItem("authUser", JSON.stringify(userData));

    setError(null);
    return true;
  };
  // Fazer logout
  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem("authToken");
    localStorage.removeItem("authUser");
    setError(null);
  };

  const isAuthenticated = !!token && isUserSecretary(user);

  const value = useMemo(
    () => ({
      user,
      token,
      isLoading,
      error,
      isAuthenticated,
      login,
      logout,
      setError,
    }),
    [user, token, isLoading, error, isAuthenticated],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider");
  }
  return context;
}
