import { create } from "zustand";
import type { FavoriteDto, TelegramUser, TopicDto } from "./types";

interface AppState {
  user: TelegramUser | null;
  query: string;
  topics: TopicDto[];
  favorites: FavoriteDto[];
  selectedTopicIds: number[];
  setUser: (user: TelegramUser | null) => void;
  setQuery: (query: string) => void;
  setTopics: (topics: TopicDto[]) => void;
  setFavorites: (favorites: FavoriteDto[]) => void;
  toggleSelected: (topicId: number) => void;
}

export const useAppStore = create<AppState>((set) => ({
  user: null,
  query: "",
  topics: [],
  favorites: [],
  selectedTopicIds: [],
  setUser: (user) => set({ user }),
  setQuery: (query) => set({ query }),
  setTopics: (topics) => set({ topics }),
  setFavorites: (favorites) => set({ favorites }),
  toggleSelected: (topicId) =>
    set((state) => ({
      selectedTopicIds: state.selectedTopicIds.includes(topicId)
        ? state.selectedTopicIds.filter((id) => id !== topicId)
        : [...state.selectedTopicIds, topicId]
    }))
}));
