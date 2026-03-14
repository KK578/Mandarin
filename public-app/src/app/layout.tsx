import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { NavBar, UserProfile } from "./components/NavBar";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "The Little Mandarin",
  description: "The Little Mandarin server-side rendered application",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased w-screen h-screen flex flex-col`}
      >
        <NavBar />
        <div className="grow overflow-y-auto">{children}</div>
      </body>
    </html>
  );
}
