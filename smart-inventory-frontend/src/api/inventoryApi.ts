import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5100/api",
});

// Get top N products
export const getProductsTop = (count: number = 5) => api.get(`/products/top?count=${count}`);

// Get paginated products
export const getProductsPaged = (
    page: number,
    pageSize: number,
    categoryId?: string,
    search?: string
) => {
    let url = `/products/paged?page=${page}&pageSize=${pageSize}`;
    if (categoryId) url += `&categoryId=${categoryId}`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    return api.get(url);
};

// Get all categories
export const getCategories = () => api.get("/categories");