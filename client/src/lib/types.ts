import colors from "@/ui/colors";
import { ReactNode } from "react";

export type ComponentType<TProps> = (
    props: TProps
) => ReactNode | Promise<ReactNode>;

export type ColorName = keyof typeof colors;

