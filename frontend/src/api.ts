import axios from "axios";
import type { FavoriteDto, TelegramAuthResponse, TopicDto } from "./types";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? "http://localhost:8080",
  timeout: 10000
});

export const apiClient = {
  async authTelegram(initData: string): Promise<TelegramAuthResponse> {
    const response = await api.post<TelegramAuthResponse>("/auth/telegram", { initData });
    return response.data;
  },
  async searchTopics(query: string): Promise<TopicDto[]> {
    const response = await api.get<TopicDto[]>("/topics", { params: { query } });
    return response.data;
  },
  async addFavorite(userId: number, topicId: number): Promise<void> {
    await api.post("/favorites", { userId, topicId });
  },
  async getFavorites(userId: number): Promise<FavoriteDto[]> {
    const response = await api.get<FavoriteDto[]>(`/favorites/${userId}`);
    return response.data;
  }
};
