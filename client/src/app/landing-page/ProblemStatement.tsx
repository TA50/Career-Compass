import typographyVariants from '@/ui/typographyVariants';
import clsx from 'clsx';
import Image from 'next/image';
import { Card, CardContent, CardHeader, CardTitle } from '@/ui/components/card';
import DocumentIcon from '@/ui/components/document-icon';
import colors from '@/ui/colors';
import { ColorName, ComponentType } from '@/lib/types';
import CupIcon from '@/ui/components/cup-icon';
import MicIcon from '@/ui/components/mic-icon';
import LikeIcon from '@/ui/components/like-icon';

function ProblemStatementImage() {
    return <div className='flex flex-row justify-end'>
        <Image
            className='w-[500px] hidden sm:block'
            alt='Problem Statement'
            src='/images/landing-page/journaling.png'
            width={2000}
            height={2000}
        />
        <Image
            className='w-[280px] block sm:hidden'
            alt='Problem Statement'
            src='/images/landing-page/journaling-small.png'
            width={1000}
            height={1000}
        />
    </div>
}

function ProblemCard(
    props: {
        title: string,
        content: string,
        icon: ComponentType<{
            className: string,
            fill: string
        }>
        color: ColorName,
    }
) {
    const colorValue = colors[props.color];

    const coloring = {
        'accent': {
            title: 'text-accent',
            border: 'border-accent'
        },
        'primary-light': {
            title: 'text-primary-light',
            border: 'border-primary-light'
        },
        'primary': {
            title: 'text-primary',
            border: 'border-primary'
        },
        'success': {
            title: 'text-success',
            border: 'border-success'
        }
    }

    const value = new Map(Object.entries(coloring)).get(props.color);

    return <Card className={`w-full md:w-[45%] mt-4 border-b-8 ${value?.border}`}>
        <CardHeader>
            <props.icon className='w-16 h-16' fill={colorValue} />
            <CardTitle className={value?.title}>
                <p className={clsx(typographyVariants.large)}>
                    {props.title}
                </p>
            </CardTitle>
        </CardHeader>
        <CardContent >
            <p className={clsx(typographyVariants.p)}>
                {props.content}
            </p>
        </CardContent>
    </Card>

}
export default function ProblemStatement() {




    return (
        <section id="problem-statement"
            className={clsx('flex flex-col items-center justify-start w-full  h-[100vh]')}
        >
            <h1 className={clsx(typographyVariants.h1, "text-primary text-center")}>Don&lsquo;t Let Your Achievements <br className='md:hidden' /> Fade Away</h1>

            <div className="flex  mt-16 flex-col md:flex-row  items-center justify-center">
                <div className="md:w-1/2">
                    <p className={clsx(typographyVariants.extraLarge, 'text-center md:text-left')}>
                        Tracking your professional growth is challenging. You achieve great things, boosting productivity, solving problems, and driving success, but over time, those accomplishments fade from memory.
                    </p>
                </div>
                <div className="md:w-1/2">
                    <ProblemStatementImage />
                </div>
            </div>

            <div className='flex flex-row flex-wrap justify-between gap-1'>
                <ProblemCard
                    title='Unstructured Career Reflection'
                    content="You've grown, but without documentation, tracking progress is difficult."
                    icon={DocumentIcon}
                    color='accent'
                />
                <ProblemCard
                    title='Forgetting Key Achievements'
                    content='You improved feature delivery time by 10%, but months later, you forget the details.'
                    icon={CupIcon}
                    color='primary-light'
                />
                <ProblemCard
                    title='Interview Struggles'
                    content="When asked about past successes, recalling specifics is tough."
                    icon={MicIcon}
                    color='primary'
                />
                <ProblemCard
                    title='Struggling To Showcase Your Impact'
                    content="You've done great work, but proving your value during reviews or promotions is challenging."
                    icon={LikeIcon}
                    color='success'
                />
            </div>
        </section>
    )
}
