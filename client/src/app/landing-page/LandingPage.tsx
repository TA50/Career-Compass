import Page from '@/ui/components/page';
import { CallToAction } from './CallToAction';
import ProblemStatement from './ProblemStatement';

export default function LandingPage() {
    return <Page>
        <CallToAction />
        <ProblemStatement />
    </Page>
}