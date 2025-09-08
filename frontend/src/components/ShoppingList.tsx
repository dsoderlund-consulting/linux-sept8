// src/components/ShoppingList.tsx
import { useState, useEffect, KeyboardEvent } from "react";
import shoppingListApi from "../api/shoppingListApi";

interface ShoppingListItem {
  id: number;
  description: string;
  isDone: boolean;
}

function ShoppingList() {
  const [items, setItems] = useState<ShoppingListItem[]>([]);
  const [newItemDescription, setNewItemDescription] = useState<string>("");
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchItems = async () => {
      try {
        const data = await shoppingListApi.getItems();
        setItems(data);
      } catch (err) {
        if (typeof err === "string") {
          setError(err || "An error occurred while fetching items.");
        } else if (err instanceof Error) {
          setError(err.message);
        }
      } finally {
        setIsLoading(false);
      }
    };

    fetchItems();
  }, []);

  const handleAddItem = async () => {
    if (newItemDescription.trim() === "") return;

    try {
      const newItem = { description: newItemDescription };
      const addedItem = await shoppingListApi.addItem(newItem);
      setItems([...items, addedItem]);
      setNewItemDescription("");
    } catch (err) {
      if (typeof err === "string") {
        setError(err || "An error occurred while adding an item.");
      } else if (err instanceof Error) {
        setError(err.message);
      }
    }
  };

  const handleToggleDone = async (item: ShoppingListItem) => {
    try {
      const updatedItem = { ...item, isDone: !item.isDone };
      await shoppingListApi.updateItem(updatedItem);
      setItems(items.map((i) => (i.id === item.id ? updatedItem : i)));
    } catch (err) {
      if (typeof err === "string") {
        setError(err || "An error occurred while updating an item.");
      } else if (err instanceof Error) {
        setError(err.message);
      }
    }
  };

  const handleDeleteItem = async (id: number) => {
    try {
      await shoppingListApi.deleteItem(id);
      setItems(items.filter((item) => item.id !== id));
    } catch (err) {
      if (typeof err === "string") {
        setError(err || "An error occurred while deleting an item.");
      } else if (err instanceof Error) {
        setError(err.message);
      }
    }
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault(); // Prevent form submission
      handleAddItem();
    }
  };

  if (isLoading) {
    return <div className="text-center">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-red-500">Error: {error}</div>;
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6 text-gray-800">Shopping List</h1>
      <div className="mb-6 flex">
        <input
          type="text"
          className="border border-gray-300 px-3 py-2 mr-2 flex-grow rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
          placeholder="Add new item"
          value={newItemDescription}
          onChange={(e) => setNewItemDescription(e.target.value)}
          onKeyDown={handleKeyDown} // Add the onKeyDown event handler
        />
        <button
          className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded-md"
          onClick={handleAddItem}
        >
          Add
        </button>
      </div>
      <ul className="divide-y divide-gray-200">
        {items.map((item) => (
          <li key={item.id} className="flex items-center justify-between py-3">
            <div
              className={`flex-grow ${
                item.isDone ? "line-through text-gray-500" : "text-gray-700"
              }`}
            >
              {item.description}
            </div>
            <div className="flex space-x-12">
              <button
                className={`px-3 py-1 rounded-md text-white ${
                  item.isDone
                    ? "bg-yellow-500 hover:bg-yellow-600"
                    : "bg-green-500 hover:bg-green-600"
                }`}
                onClick={() => handleToggleDone(item)}
              >
                {item.isDone ? "Undone" : "Done"}
              </button>
              <button
                className="bg-red-500 hover:bg-red-600 text-white px-3 py-1 rounded-md"
                onClick={() => handleDeleteItem(item.id)}
              >
                Delete
              </button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ShoppingList;
