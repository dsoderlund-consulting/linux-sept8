// src/App.tsx
import ShoppingList from "./components/ShoppingList";

function App() {
  return (
    <div className="App min-h-screen flex items-center justify-center bg-gray-100">
      <div className="container p-8 max-w-sm bg-white rounded-lg shadow-md">
        <ShoppingList />
      </div>
    </div>
  );
}

export default App;
