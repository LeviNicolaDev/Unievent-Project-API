import AsyncStorage from "@react-native-async-storage/async-storage";
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

import { authApi, getApiAssetUrl, studentsApi } from "../services/api";

const AuthContext = createContext(null);

const STORAGE_KEYS = {
  token: "@unievent:auth-token",
  student: "@unievent:student",
};

function getStudentNameFromEmail(email) {
  return String(email || "").split("@")[0] || "Aluno";
}

async function findStudentByEmail(email) {
  try {
    const students = await studentsApi.list();
    return students.find(
      (student) =>
        String(student.email || student.Email).toLowerCase() ===
        String(email).toLowerCase()
    );
  } catch {
    return null;
  }
}

function normalizeStudent(student, fallbackEmail) {
  if (!student) {
    return {
      email: fallbackEmail,
      nome: getStudentNameFromEmail(fallbackEmail),
    };
  }

  return {
    id: student.id ?? student.Id,
    nome: student.nome ?? student.Nome ?? getStudentNameFromEmail(fallbackEmail),
    email: student.email ?? student.Email ?? fallbackEmail,
    fotoPerfil: getApiAssetUrl(student.fotoPerfil ?? student.FotoPerfil),
    dataNascimento: student.dataNascimento ?? student.DataNascimento,
    role: student.role ?? student.Role,
  };
}

export function AuthProvider({ children }) {
  const [token, setToken] = useState(null);
  const [student, setStudent] = useState(null);
  const [hydrated, setHydrated] = useState(false);

  useEffect(() => {
    let isMounted = true;

    async function hydrate() {
      try {
        const [storedToken, storedStudent] = await Promise.all([
          AsyncStorage.getItem(STORAGE_KEYS.token),
          AsyncStorage.getItem(STORAGE_KEYS.student),
        ]);

        if (!isMounted) return;

        setToken(storedToken);
        setStudent(storedStudent ? JSON.parse(storedStudent) : null);
      } catch {
        return null;
      } finally {
        if (isMounted) {
          setHydrated(true);
        }
      }
    }

    hydrate();

    return () => {
      isMounted = false;
    };
  }, []);

  const persistSession = useCallback(async (nextToken, nextStudent) => {
    setToken(nextToken);
    setStudent(nextStudent);

    await Promise.all([
      AsyncStorage.setItem(STORAGE_KEYS.token, nextToken),
      AsyncStorage.setItem(STORAGE_KEYS.student, JSON.stringify(nextStudent)),
    ]);
  }, []);

  const signIn = useCallback(
    async ({ email, senha }) => {
      const authToken = await authApi.loginAluno({ email, senha });
      const apiStudent = await findStudentByEmail(email);
      const nextStudent = normalizeStudent(apiStudent, email);

      await persistSession(authToken, nextStudent);

      return nextStudent;
    },
    [persistSession]
  );

  const signUp = useCallback(async (data) => {
    const createdStudent = await studentsApi.create(data);
    return normalizeStudent(createdStudent, data.email);
  }, []);

  const updateProfile = useCallback(
    async (data) => {
      if (!student?.id) {
        throw new Error("Aluno não encontrado para edição.");
      }

      const updatedStudent = await studentsApi.update(student.id, data, token);
      const nextStudent = normalizeStudent(updatedStudent, student.email);

      setStudent(nextStudent);
      await AsyncStorage.setItem(
        STORAGE_KEYS.student,
        JSON.stringify(nextStudent)
      );

      return nextStudent;
    },
    [student, token]
  );

  const signOut = useCallback(async () => {
    setToken(null);
    setStudent(null);

    await Promise.all([
      AsyncStorage.removeItem(STORAGE_KEYS.token),
      AsyncStorage.removeItem(STORAGE_KEYS.student),
    ]);
  }, []);

  const value = useMemo(
    () => ({
      hydrated,
      isAuthenticated: Boolean(token),
      signIn,
      signOut,
      signUp,
      student,
      token,
      updateProfile,
    }),
    [hydrated, signIn, signOut, signUp, student, token, updateProfile]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider");
  }

  return context;
}
