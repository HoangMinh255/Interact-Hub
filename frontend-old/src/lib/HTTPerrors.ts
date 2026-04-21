export class HttpError extends Error {
  status: number;
  payload: {
    message: string,
    [key: string]: any
  };
  constructor({ status, payload }) {
    super("HTTP Error");
    this.status = status;
    this.payload = payload;
  }
}
