import { useState } from "react";


export function useLocalStorage<T>(key: string, initialValue: T) : [T, (value: T | ((val: T) => T)) => void]{


	const [storeValue, setStoreValue] = useState<T>(() => {
		try{
			const item = window.localStorage.getItem(key);
			
			return item ? JSON.parse(item) : initialValue;
		} catch(error) {
			console.log(error);
			return initialValue;
		}
	});

	const setValue = (value: T | ((val: T) => T)) => {
		try{
			const valueToStore = value instanceof Function ? value(storeValue) : value;

			setStoreValue(valueToStore);
			window.localStorage.setItem(key, JSON.stringify(valueToStore));
		} catch(error) {
			console.log(error);
		}
	};

	return [storeValue,  setValue];
}