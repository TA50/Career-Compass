const config = {
    isDevelopment: process.env.NODE_ENV?.trim().toLowerCase() === "development",
    isProduction: process.env.NODE_ENV?.trim().toLowerCase() === "production",
} as const;

export default config;

