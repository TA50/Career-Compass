import chalk from "chalk";

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const info = (...params: any[]) => {
    console.log(chalk.blue(...params));
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const error = (...params: any[]) => {
    console.error(chalk.red(...params));
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const success = (...params: any[]) => {
    console.log(chalk.green(...params));
};

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const warning = (...params: any[]) => {
    console.warn(chalk.yellow(...params));
};

