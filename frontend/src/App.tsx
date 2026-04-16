import { useEffect, useMemo, useState } from "react";
import { apiClient } from "./api";
import { useAppStore } from "./store";
import { getInitData, initTelegram, sendDataToBot, tg } from "./telegram";

type View = "home" | "favorites";

export function App() {
  const [view, setView] = useState<View>("home");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { user, query, topics, favorites, selectedTopicIds, setUser, setQuery, setTopics, setFavorites, toggleSelected } =
    useAppStore();

  useEffect(() => {
    initTelegram();
    const doAuth = async () => {
      const initData = getInitData();
      if (!initData) {
        return;
      }
      const response = await apiClient.authTelegram(initData);
      setUser(response.user);
    };
    doAuth().catch(() => setError("Не удалось авторизовать Telegram-пользователя"));
  }, [setUser]);

  useEffect(() => {
    if (!user) return;
    apiClient.getFavorites(user.id).then(setFavorites).catch(() => setError("Ошибка загрузки избранного"));
  }, [user, setFavorites]);

  useEffect(() => {
    const mainButton = tg?.MainButton;
    if (!mainButton) {
      return;
    }

    const onMainButtonClick = () => {
      sendDataToBot({ selectedTopicIds, userId: user?.id });
    };
    mainButton.setText("Отправить в бота");
    mainButton.onClick(onMainButtonClick);
    if (selectedTopicIds.length > 0) {
      mainButton.show();
    } else {
      mainButton.hide();
    }
    return () => {
      mainButton.offClick(onMainButtonClick);
    };
  }, [selectedTopicIds, user?.id]);

  const favoriteTopicIds = useMemo(() => new Set(favorites.map((f) => f.topicId)), [favorites]);

  const handleSearch = async () => {
    setLoading(true);
    setError(null);
    try {
      setTopics(await apiClient.searchTopics(query));
    } catch {
      setError("Ошибка поиска тем");
    } finally {
      setLoading(false);
    }
  };

  const addFavorite = async (topicId: number) => {
    if (!user) return;
    await apiClient.addFavorite(user.id, topicId);
    setFavorites(await apiClient.getFavorites(user.id));
  };

  return (
    <main className="mx-auto min-h-screen max-w-md bg-slate-50 p-4 text-slate-900">
      <h1 className="mb-4 text-2xl font-bold">TopicFlow</h1>
      <div className="mb-4 flex gap-2">
        <button className="rounded bg-blue-600 px-3 py-2 text-white" onClick={() => setView("home")}>Поиск</button>
        <button className="rounded bg-slate-700 px-3 py-2 text-white" onClick={() => setView("favorites")}>Избранное</button>
      </div>

      {error && <p className="mb-3 rounded bg-red-100 p-2 text-sm text-red-700">{error}</p>}

      {view === "home" && (
        <>
          <div className="mb-3 flex gap-2">
            <input
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              placeholder="Введите тему"
              className="w-full rounded border px-3 py-2"
            />
            <button onClick={handleSearch} className="rounded bg-emerald-600 px-3 py-2 text-white">
              Найти
            </button>
          </div>
          {loading && <p className="text-sm text-slate-600">Поиск...</p>}
          <ul className="space-y-2">
            {topics.map((topic) => (
              <li key={topic.id} className="rounded border bg-white p-3">
                <div className="mb-1 flex items-center justify-between gap-2">
                  <h3 className="font-semibold">{topic.title}</h3>
                  <input type="checkbox" checked={selectedTopicIds.includes(topic.id)} onChange={() => toggleSelected(topic.id)} />
                </div>
                <p className="text-sm text-slate-600">{topic.description}</p>
                {!favoriteTopicIds.has(topic.id) && (
                  <button className="mt-2 text-sm text-blue-700" onClick={() => addFavorite(topic.id)}>
                    Добавить в избранное
                  </button>
                )}
              </li>
            ))}
          </ul>
        </>
      )}

      {view === "favorites" && (
        <ul className="space-y-2">
          {favorites.map((item) => (
            <li key={item.id} className="rounded border bg-white p-3">
              <h3 className="font-semibold">{item.topic.title}</h3>
              <p className="text-sm text-slate-600">{item.topic.description}</p>
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}
