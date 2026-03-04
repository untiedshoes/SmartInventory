import React, { useState, useEffect } from 'react';
import { getProductsPaged, getCategories } from '../api/inventoryApi';

interface Category {
    id: string;
    name: string;
    description?: string;
}

interface Product {
    id: string;
    name: string;
    description?: string;
    quantity: number;
    price: number;
    categoryId: string;
    categoryName?: string;
}

interface PagedResponse {
    data: Product[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

const ProductsPage: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [selectedCategoryId, setSelectedCategoryId] = useState<string | null>(null);
    const [searchTerm, setSearchTerm] = useState<string>('');
    const [page, setPage] = useState<number>(1);
    const [pageSize] = useState<number>(20);
    const [totalPages, setTotalPages] = useState<number>(1);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    // Fetch categories for dropdown
    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const res = await getCategories();
                setCategories(res.data);
            } catch (err: any) {
                console.error(err);
            }
        };
        fetchCategories();
    }, []);

    // Fetch products (paged)
    useEffect(() => {
        const fetchProducts = async () => {
            setLoading(true);
            try {
                const res = await getProductsPaged(
                    page,
                    pageSize,
                    selectedCategoryId || undefined,
                    searchTerm || undefined
                );
                const paged: PagedResponse = res.data;

                // Add category name
                const productsWithCategory = paged.data.map(p => ({
                    ...p,
                    categoryName: categories.find(c => c.id === p.categoryId)?.name
                }));

                setProducts(productsWithCategory);
                setTotalPages(paged.totalPages);
                setError(null);
            } catch (err: any) {
                setError(err.message || 'Failed to fetch products');
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, [page, pageSize, selectedCategoryId, searchTerm, categories]);

    return (
        <div style={{ marginTop: '1rem' }}>
            {/* Filters */}
            <div style={{ marginBottom: '1rem' }}>
                <label>
                    Category:{' '}
                    <select
                        value={selectedCategoryId || ''}
                        onChange={e => {
                            setSelectedCategoryId(e.target.value || null);
                            setPage(1);
                        }}
                    >
                        <option value="">All Categories</option>
                        {categories.map(c => (
                            <option key={c.id} value={c.id}>
                                {c.name}
                            </option>
                        ))}
                    </select>
                </label>

                <label style={{ marginLeft: '1rem' }}>
                    Search:{' '}
                    <input
                        type="text"
                        value={searchTerm}
                        onChange={e => {
                            setSearchTerm(e.target.value);
                            setPage(1);
                        }}
                        placeholder="Search products..."
                    />
                </label>
            </div>

            {/* Loading / Error */}
            {loading && <p>Loading products...</p>}
            {error && <p style={{ color: 'red' }}>Error: {error}</p>}

            {/* Products Table */}
            {!loading && !error && (
                <>
                    <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr>
                                <th style={{ textAlign: 'left' }}>Name</th>
                                <th style={ {textAlign: 'left'}}>Description</th>
                                <th style={{ textAlign: 'right' }}>Quantity</th>
                                <th style={{ textAlign: 'right' }}>Price</th>
                                <th style={{ textAlign: 'right' }}>Category</th>
                            </tr>
                        </thead>
                        <tbody>
                            {products.map(p => (
                                <tr key={p.id}>
                                    <td style={{ textAlign: 'left' }}>{p.name}</td>
                                    <td style={{ textAlign: 'left' }}>{p.description}</td>
                                    <td style={{ textAlign: 'right' }}>{p.quantity}</td>
                                    <td style={{ textAlign: 'right' }}>£{p.price.toFixed(2)}</td>
                                    <td style={{ textAlign: 'right' }}>{p.categoryName}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {/* Pagination */}
                    <div style={{ marginTop: '1rem' }}>
                        <button onClick={() => setPage(prev => Math.max(prev - 1, 1))} disabled={page === 1}>
                            Previous
                        </button>
                        <span style={{ margin: '0 1rem' }}>Page {page} of {totalPages}</span>
                        <button onClick={() => setPage(prev => Math.min(prev + 1, totalPages))} disabled={page === totalPages}>
                            Next
                        </button>
                    </div>
                </>
            )}
        </div>
    );
};

export default ProductsPage;