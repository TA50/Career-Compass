import type { Metadata } from "next";
import "./globals.css";
import { rubik } from './fonts';

export const metadata: Metadata = {
  title: "Career Compass",
  description: "This personal web application is designed to help professionals effectively prepare for job interviews by organizing real-life experiences in the STAR (Situation, Task, Action, Result) format. The app serves as your companion to build and manage behavioral examples, track progress, and streamline interview preparation.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${rubik.className} antialiased`}
      >
        {children}
      </body>
    </html>
  );
}
