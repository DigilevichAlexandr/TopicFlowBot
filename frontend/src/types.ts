export interface TelegramUser {
  id: number;
  username?: string;
  first_name?: string;
}

export interface TopicDto {
  id: number;
  title: string;
  description: string;
}

export interface FavoriteDto {
  id: number;
  userId: number;
  topicId: number;
  topic: TopicDto;
}

export interface TelegramAuthResponse {
  user: TelegramUser;
}
