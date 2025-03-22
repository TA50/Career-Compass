

import { Button } from '@/ui/components/button';
import config from '@/lib/config';
import { notFound } from 'next/navigation';
import Logo from '@/ui/components/logo';
import MicIcon from '@/ui/components/mic-icon';
import LikeIcon from '@/ui/components/like-icon';
import CupIcon from '@/ui/components/cup-icon';
import DocumentIcon from '@/ui/components/document-icon';
import colors from '@/ui/colors';
export default function Dev() {
    if (!config.isDevelopment) {
        notFound();
    }
    return (

        <main className='flex flex-col gap-y-8 items-center justify-center min-h-screen py-2'>
            <h1>Buttons</h1>
            <section className='flex flex-wrap flex-row gap-8 row-start-2 items-center sm:items-start'>
                <Button variant='primary'>primary</Button>
                <Button variant='accent'>accent</Button>
                <Button variant='secondary'>secondary</Button>
                <Button variant='outline'>outline</Button>
                <Button variant='text'>text</Button>
                <Button variant='destructive'>destructive</Button>
                <Button variant='link'>link</Button>
            </section>

            <h1>Typography</h1>
            <section className='flex flex-wrap flex-row gap-8 row-start-2 items-center sm:items-start'>
                <h1 className='text-4xl font-extrabold tracking-tight lg:text-5xl'>h1</h1>
                <h2 className='border-b pb-2 text-3xl font-semibold tracking-tight transition-colors first:mt-0'>h2</h2>
                <h3 className='text-2xl font-semibold tracking-tight'>h3</h3>
                <h4 className='text-xl font-semibold tracking-tight'>h4</h4>
                <p className='leading-7'>p</p>
                <blockquote className='mt-6 border-l-2 pl-6 italic'>blockquote</blockquote>
                <code className='relative rounded bg-muted px-[0.3rem] py-[0.2rem] font-mono text-sm font-semibold'>inlineCode</code>
                <p className='text-xl text-muted-foreground'>lead</p>
                <p className='text-lg font-semibold'>large</p>
                <p className='text-sm font-medium leading-none'>small</p>
                <p className='text-sm text-muted-foreground'>muted</p>
            </section>



            <h1>Icons</h1>
            <section className='flex flex-wrap flex-row gap-8'>
                <Logo
                    className='w-12 h-12'
                    fill='hsl(var(--primary))' />


                <MicIcon fill='hsl(var(--primary))'
                    className='w-12 h-12'
                />

                <LikeIcon fill='hsl(var(--primary))'
                    className='w-12 h-12'
                />

                <CupIcon fill='hsl(var(--primary-light))'
                    className='w-12 h-12'
                />

                <DocumentIcon fill={colors.primary}
                    className='w-12 h-12'
                />
            </section>

        </main>

    );
}
