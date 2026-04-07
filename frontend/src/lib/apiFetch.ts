import { HttpError } from "./HTTPerrors";

type EntityErrorPayLoad = {
  message: string;
  errors: {
    field: string;
    message: string;
  }[];
};

export class EntityError extends HttpError {
  constructor({
    status,
    payload,
  }: {
    status: 422;
    payload: EntityErrorPayLoad;
  }) {
    super({ status, payload });
  }
}

export const isClient = () => typeof window !== "undefined";
let clientLogoutRequest: null | Promise<any> = null;

export async function apiFetch<Response>(
  method: "GET" | "POST" | "PUT" | "DELETE",
  url: string,
  options?: RequestInit | undefined
) {
  const body = options?.body ? JSON.stringify(options.body) : undefined;
  const baseHeader: {
    [key: string]: string;
  } = {
    "Content-Type": "application/json",
  };
  const accessToken = localStorage.getItem("accessToken");
  if (accessToken) {
    baseHeader.Authorization = `Bearer ${accessToken}`;
  }
  const baseUrl = import.meta.env.VITE_API_URL;
  const fullUrl = url.startsWith("/")
    ? `${baseUrl}${url}`
    : `${baseUrl}/${url}`;
  const res = await fetch(fullUrl, {
    ...options,
    headers: {
      ...baseHeader,
      ...options?.headers,
    },
    body,
    method,
  });
  const payload: Response = await res.json();
  const data = {
    status: res.status,
    payload: payload,
  };
  console.log(data);
  if (!res.ok) {
    if (res.status === 401) {
      if (isClient()) {
        if (!clientLogoutRequest) {
          clientLogoutRequest = fetch("/api/auth/logout", {
            method: "POST",
            body: JSON.stringify({ force: true }),
            headers: {
              ...baseHeader,
            },
          });
          try {
            await clientLogoutRequest;
          } catch (error : any) {
            console.log(error);
          } finally {
            localStorage.removeItem("accessToken");
            localStorage.removeItem("accessTokenExpiresAt");
            clientLogoutRequest = null;
            location.href = "/login";
          }
        }
      }
    } else if (res.status === 422) {
      console.log("ada");
      throw new EntityError(
        data as {
          status: 422;
          payload: EntityErrorPayLoad;
        }
      );
    } else throw new HttpError(data);
  }
  return data;
}
const http = {
  get<Response>(url: string, options?: Omit<RequestInit, "body"> | undefined) {
    return apiFetch<Response>("GET", url, options);
  },
  post<Response>(
    url: string,
    body: any,
    options?: Omit<RequestInit, "body"> | undefined
  ) {
    return apiFetch<Response>("POST", url, { ...options, body });
  },
  put<Response>(
    url: string,
    body: any,
    options?: Omit<RequestInit, "body"> | undefined
  ) {
    return apiFetch<Response>("PUT", url, { ...options, body });
  },
  delete<Response>(
    url: string,
    body: any,
    options?: Omit<RequestInit, "body"> | undefined
  ) {
    return apiFetch<Response>("DELETE", url, { ...options, body });
  },
};

export default http;
