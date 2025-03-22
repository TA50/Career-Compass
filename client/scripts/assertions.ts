import { error } from "./logs";
const anyAssertion = <T>(value: T) => ({
    is: (expected: T, message?: string) => {
        if (value !== expected) {
            error(message || `Expected ${value} to be ${expected}`);
            process.exit(1);
        }
    },
    oneOf: (expected: T[], message?: string) => {
        if (!expected.includes(value)) {
            error(
                message ||
                    `Expected ${value} to be one of ${expected.join(", ")}`
            );
            process.exit(1);
        }
    },
});

const numberAssertion = (value: number) => ({
    ...anyAssertion<number>(value),
    isGreaterOrEqualThan: (expected: number, message?: string) => {
        if (value <= expected) {
            error(
                message || `Expected ${value} to be greater than ${expected}`
            );
            process.exit(1);
        }
    },
    isLessOrEqualThan: (expected: number, message?: string) => {
        if (value >= expected) {
            error(message || `Expected ${value} to be less than ${expected}`);
            process.exit(1);
        }
    },
    isInRange: (min: number, max: number, message?: string) => {
        if (value < min || value > max) {
            error(
                message || `Expected ${value} to be between ${min} and ${max}`
            );
            process.exit(1);
        }
    },
});
const stringAssertion = (value: string) => ({
    ...anyAssertion<string>(value),
    matches: (regex: RegExp, message?: string) => {
        if (!regex.test(value)) {
            error(message || `Expected ${value} to match ${regex}`);
            process.exit(1);
        }
    },
    hasLength: (length: number, message?: string) => {
        if (value.length !== length) {
            error(message || `Expected ${value} to have length of ${length}`);
            process.exit(1);
        }
    },
    hasMaxLength: (maxLength: number, message?: string) => {
        if (value.length > maxLength) {
            error(
                message ||
                    `Expected ${value} to have max length of ${maxLength}`
            );
            process.exit(1);
        }
    },
    hasMinLength: (minLength: number, message?: string) => {
        if (value.length < minLength) {
            error(
                message ||
                    `Expected ${value} to have min length of ${minLength}`
            );
            process.exit(1);
        }
    },
});
const booleanAssertion = (value: boolean) => ({
    ...anyAssertion<boolean>(value),
    isTrue: (message?: string) => {
        if (!value) {
            error(message || `Expected ${value} to be true`);
            process.exit(1);
        }
    },
    isFalse: (message?: string) => {
        if (value) {
            error(message || `Expected ${value} to be false`);
            process.exit(1);
        }
    },
});

export function assert(value: boolean): ReturnType<typeof booleanAssertion>;
export function assert(value: number): ReturnType<typeof numberAssertion>;
export function assert(value: string): ReturnType<typeof stringAssertion>;
export function assert(value: unknown): ReturnType<typeof anyAssertion>;
export function assert<T>(value: T) {
    if (typeof value === "number") {
        return numberAssertion(value);
    }

    if (typeof value === "string") {
        return stringAssertion(value);
    }

    if (typeof value === "boolean") {
        return booleanAssertion(value);
    }
    return anyAssertion(value);
}

