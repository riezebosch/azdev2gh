import React from 'react';
import { render, fireEvent, wait, screen, waitForElementToBeRemoved, queryByTestId } from '@testing-library/react';
import App from './App';
import axios from 'axios'

jest.mock('axios')
const axiosMock = axios as jest.Mocked<typeof axios>;

test('invokes api on button click', async () => {
  const { getByLabelText, getByRole, getByText } = render(<App />);  
  
  axiosMock.post.mockResolvedValueOnce({
    data: { greeting: 'hello there' },
  })
  
  fireEvent.change(getByLabelText('azure devops token *'), { target: { value: 'xxx' }});
  fireEvent.change(getByLabelText('organization *'), { target: { value: 'test' }});
  fireEvent.change(getByLabelText('area path *'), { target: { value: 'test/team A' }});
  fireEvent.change(getByLabelText('github token *'), { target: { value: 'yyy' }});
  fireEvent.click(getByRole('button'));

  await wait(() => getByRole('button'));
  expect(axiosMock.post).toHaveBeenCalledWith(expect.any(String), {
    azureDevOps: {
      token: 'xxx',
      organization: 'test',
      areaPath: 'test/team A'
    },
    github: {
      token: 'yyy'
    }
  });
});

test('show snackbar on post error', async () => {
  const { getByLabelText, getByRole, getByText } = render(<App />);  
  
  axiosMock.post.mockRejectedValue('something wrong');

  fireEvent.change(getByLabelText('azure devops token *'), { target: { value: 'xxx' }});
  fireEvent.change(getByLabelText('organization *'), { target: { value: 'test' }});
  fireEvent.change(getByLabelText('area path *'), { target: { value: 'test/team A' }});
  fireEvent.change(getByLabelText('github token *'), { target: { value: 'yyy' }});
  fireEvent.click(getByRole('button'));

  await wait(() => getByText('something wrong'));
});

test('show function error', async () => {
  const { getByLabelText, getByRole, getByText } = render(<App />);  
  
  axiosMock.post.mockResolvedValueOnce({ 
    data: {
      "name":"Migrate",
      "instanceId":"0535fdb8b11b45f08f91b01975cd905d",
      "runtimeStatus":"Failed",
      "input":{"AzureDevOps":{"Organization":"","Token":"","AreaPath":""},"GitHub":{"Token":""}},
      "customStatus":null,
      "output":"Orchestrator function 'Migrate' failed: some error"
    }
  });

  fireEvent.change(getByLabelText('azure devops token *'), { target: { value: 'xxx' }});
  fireEvent.change(getByLabelText('organization *'), { target: { value: 'test' }});
  fireEvent.change(getByLabelText('area path *'), { target: { value: 'test/team A' }});
  fireEvent.change(getByLabelText('github token *'), { target: { value: 'yyy' }});
  fireEvent.click(getByRole('button'));

  await wait(() => getByText('function failed'));
});

test('show loading', async () => {
  const { getByLabelText, getByRole, getByTestId, queryByTestId } = render(<App />);  

  axiosMock.post.mockResolvedValue({ 
    data: null
  });

  fireEvent.change(getByLabelText('azure devops token *'), { target: { value: 'xxx' }});
  fireEvent.change(getByLabelText('organization *'), { target: { value: 'test' }});
  fireEvent.change(getByLabelText('area path *'), { target: { value: 'test/team A' }});
  fireEvent.change(getByLabelText('github token *'), { target: { value: 'yyy' }});
  fireEvent.click(getByRole('button'));

  await wait(() => getByTestId('loading'));
  expect(queryByTestId('error')).toBe(null);  

  await waitForElementToBeRemoved(() => getByTestId('loading'));
});

test('go button disabled while required input empty', async () => {
  const { getByLabelText, getByRole } = render(<App />);  
  
  fireEvent.change(getByLabelText('azure devops token *'), { target: { value: 'xxx' }});
  expect(getByRole('button')).toHaveAttribute("disabled");

  fireEvent.change(getByLabelText('organization *'), { target: { value: 'test' }});
  expect(getByRole('button')).toHaveAttribute("disabled");

  fireEvent.change(getByLabelText('area path *'), { target: { value: 'test/team A' }});
  expect(getByRole('button')).toHaveAttribute("disabled");

  fireEvent.change(getByLabelText('github token *'), { target: { value: 'yyy' }});
  expect(getByRole('button')).not.toHaveAttribute("disabled");
});
