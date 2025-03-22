import { Button } from '@/ui/components/button';
import LogoSVG from '@/ui/components/logo';
import typographyVariants from '@/ui/typographyVariants';
import clsx from 'clsx';
import Link from "next/link"

function Logo() {
    return <div className='lg:size-48 size-36 sm:mt-16 mt-2 mb-4'>
        <LogoSVG
            fill='hsl(var(--primary))' />
    </div>
}


export function CallToAction() {



    return <section id="call-to-action"
        className={clsx('flex flex-col items-center justify-start w-full h-[100vh]')}
    >
        <Logo />
        <h1 className={clsx(typographyVariants.h1, "text-primary capitalize text-center")}>Shape your future</h1>
        <h2 className={clsx(typographyVariants.h2, "text-primary capitalize text-center")}>Begin your jouney today</h2>
        <p className={clsx(typographyVariants.extraLarge, 'text-center mt-16')}>Career Compass helps you track your journey, showcase achievements, and prepare for the future with confidence.</p>
        <Button
            asChild
            className='mt-6 md:mt-8' size='lg'>
            <Link href="/login">Get Started</Link>
        </Button>

    </section>
}