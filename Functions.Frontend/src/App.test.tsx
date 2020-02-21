import React from 'react';
import { render, fireEvent, wait } from '@testing-library/react';
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
