import React from "react";
import {
  render,
  waitFor,
  waitForElementToBeRemoved} from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import App from "./App";
import axios from "axios";

jest.mock("axios");
const axiosMock = axios as jest.Mocked<typeof axios>;

describe("app", () => {
  it("invokes api on button click", async () => {
    const { getByLabelText, getByRole } = render(<App />);

    axiosMock.post.mockResolvedValueOnce({
      data: { greeting: "hello there" }
    });

    userEvent.type(getByLabelText("azure devops token *"), "xxx");
    userEvent.type(getByLabelText("organization *"), "test");
    userEvent.type(getByLabelText("area path *"), "test/team A");
    userEvent.type(getByLabelText("github token *"), "yyy");
    userEvent.click(getByRole("button"));

    await waitFor(() => getByRole("button"));
    expect(axiosMock.post).toHaveBeenCalledWith(expect.any(String), {
      azureDevOps: {
        token: "xxx",
        organization: "test",
        areaPath: "test/team A"
      },
      github: {
        token: "yyy"
      }
    });
  });

  it("shows snackbar on post error", async () => {
    const { getByLabelText, getByRole, getByText } = render(<App />);

    axiosMock.post.mockRejectedValue("something wrong");

    userEvent.type(getByLabelText("azure devops token *"), "xxx");
    userEvent.type(getByLabelText("organization *"), "test");
    userEvent.type(getByLabelText("area path *"), "test/team A");
    userEvent.type(getByLabelText("github token *"), "yyy");
    userEvent.click(getByRole("button"));

    await waitFor(() => getByText("something wrong"));
  });

  it("shows function error when runtime status is failed", async () => {
    const { getByLabelText, getByRole, getByText } = render(<App />);

    axiosMock.post.mockResolvedValueOnce({
      data: {
        name: "Migrate",
        instanceId: "0535fdb8b11b45f08f91b01975cd905d",
        runtimeStatus: "Failed",
        input: {
          AzureDevOps: { Organization: "", Token: "", AreaPath: "" },
          GitHub: { Token: "" }
        },
        customStatus: null,
        output: "Orchestrator function 'Migrate' failed: some error"
      }
    });

    userEvent.type(getByLabelText("azure devops token *"), "xxx");
    userEvent.type(getByLabelText("organization *"), "test");
    userEvent.type(getByLabelText("area path *"), "test/team A");
    userEvent.type(getByLabelText("github token *"), "yyy");
    userEvent.click(getByRole("button"));

    await waitFor(() => getByText("function failed"));
  });

  it("shows loading component while awaiting the api response", async () => {
    const { getByLabelText, getByRole, getByTestId, queryByTestId } = render(
      <App />
    );

    axiosMock.post.mockResolvedValue({
      data: null
    });

    userEvent.type(getByLabelText("azure devops token *"), "xxx");
    userEvent.type(getByLabelText("organization *"), "test");
    userEvent.type(getByLabelText("area path *"), "test/team A");
    userEvent.type(getByLabelText("github token *"), "yyy");
    userEvent.click(getByRole("button"));

    await waitFor(() => getByTestId("loading"));
    expect(queryByTestId("error")).toBe(null);

    await waitForElementToBeRemoved(() => getByTestId("loading"));
  });

  it("disables go button until all required fields have value", async () => {
    const { getByLabelText, getByRole } = render(<App />);

    userEvent.type(getByLabelText("azure devops token *"), "xxx");
    expect(getByRole("button")).toHaveAttribute("disabled");

    userEvent.type(getByLabelText("organization *"), "test");
    expect(getByRole("button")).toHaveAttribute("disabled");

    userEvent.type(getByLabelText("area path *"), "test/team A");
    expect(getByRole("button")).toHaveAttribute("disabled");

    userEvent.type(getByLabelText("github token *"), "yyy");
    expect(getByRole("button")).not.toHaveAttribute("disabled");
  });

  it("updates link to organization for azure devops token", async () => {
    const { getByLabelText, getByTestId } = render(<App />);

    userEvent.type(getByLabelText("organization *"), "test");
    expect(getByTestId("azure-devops-token-url")).toHaveAttribute(
      "href",
      "https://dev.azure.com/test/_usersSettings/tokens"
    );
  });
});
