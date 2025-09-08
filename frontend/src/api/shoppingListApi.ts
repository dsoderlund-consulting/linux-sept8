// src/api/shoppingListApi.ts
import axios from "axios";

const API_PATH = (import.meta.env.VITE_API_BASE_URL || "") + "/api/items";

interface ShoppingListItem {
  id: number;
  description: string;
  isDone: boolean;
}

const shoppingListApi = {
  async getItems(): Promise<ShoppingListItem[]> {
    const response = await axios.get(API_PATH);
    return response.data;
  },

  async getItem(id: number): Promise<ShoppingListItem> {
    const response = await axios.get(`${API_PATH}/${id}`);
    return response.data;
  },

  async addItem(item: { description: string }): Promise<ShoppingListItem> {
    const response = await axios.post(API_PATH, item);
    return response.data;
  },

  async updateItem(item: ShoppingListItem): Promise<ShoppingListItem> {
    const response = await axios.put(`${API_PATH}/${item.id}`, item);
    return response.data;
  },

  async deleteItem(id: number): Promise<void> {
    await axios.delete(`${API_PATH}/${id}`);
  },
};

export default shoppingListApi;
