import { render } from "@testing-library/react";
import LandingPage from "@/app/landing-page/LandingPage";

it("Should match the snapshot", () => {
    const { container } = render(<LandingPage />);
    expect(container).toMatchSnapshot();
});

