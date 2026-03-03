import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5100/api",
});

// Fetch all products (deprecated, use getProducts with pagination)
export const getAll = async () => api.get("/products");

// Fetch products with pagination, category filter, search
export const getProducts = async (
    page = 1,
    pageSize = 20,
    categoryId?: string,
    search?: string
) => {
    const params: any = { page, pageSize };
    if (categoryId) params.categoryId = categoryId;
    if (search) params.search = search;

    const response = await api.get("/products", { params });
    return response.data; // PagedResponse from backend
};

// Create a new product
export const createProduct = async (product: any) => api.post("/products", product);

// Optional: fetch categories
export const getCategories = async () => api.get("/categories");