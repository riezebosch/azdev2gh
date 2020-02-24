import React, { useState } from 'react';
import './App.css';
import axios from 'axios';
import { Button, TextField, Card, Snackbar, CircularProgress } from '@material-ui/core'
import { Alert } from '@material-ui/lab'

function App() {
  const [azureDevopsToken, setAzureDevopsToken] = useState<string>('');
  const [azureDevOpsOrganization, setAzureDevOpsOrganization] = useState<string>('');
  const [azureDevOpsAreaPath, setAzureDevOpsAreaPath] = useState<string>('');
  const [gitHubToken, setGitHubToken] = useState<string>('');
  const [loading, setLoading] = React.useState<boolean>(false);
  const [error, setError] = React.useState('');

  return (
    <div className="App">
      <header className="App-header">
        <Card>
          <form style={{ padding: 15, width: 400 }} >
          <div>
            <TextField 
              label="azure devops token" 
              id="azure-devops-token"
              value={azureDevopsToken}
              onChange={x => setAzureDevopsToken(x.currentTarget.value) }
              required={true}
              fullWidth
              helperText="scope: Work Item (Read)" />
            </div>
            <div>
              <TextField 
                label="organization"
                id="azure-devops-organization"
                value={azureDevOpsOrganization}
                required={true}
                fullWidth
                onChange={x => setAzureDevOpsOrganization(x.currentTarget.value) } />
            </div>
            <div>
              <TextField
                label="area path"
                id="azure-devops-areapath"
                value={azureDevOpsAreaPath}
                required={true}
                fullWidth
                onChange={x => setAzureDevOpsAreaPath(x.currentTarget.value) } />
            </div>
            <div>
              <TextField 
                label="github token" 
                id="github-token"
                value={gitHubToken}
                required={true}
                fullWidth
                onChange={x => setGitHubToken(x.currentTarget.value) }
                helperText="scope: public_repo" />
            </div>
            <div>
              <Button disabled={ loading || [azureDevopsToken, azureDevOpsOrganization, azureDevOpsAreaPath, gitHubToken].includes('') } onClick={() => {
                setLoading(true);
                axios.post('https://azdo2gh.azurewebsites.net/api/migrate', { 
                  azureDevOps: {
                    token: azureDevopsToken,
                    organization: azureDevOpsOrganization,
                    areaPath: azureDevOpsAreaPath
                  },
                  github: {
                    token: gitHubToken
                  }
                })
                .then(r => r.data?.runtimeStatus === 'Failed' ? setError('function failed') : setError(''))
                .catch(x => setError(x.toString()))
                .finally(() => setLoading(false))
              }}>Go!</Button>    
          </div>
          </form>
        </Card>
        <Snackbar open={error !== ''} autoHideDuration={6000} onClose={ () => setError('') } data-testid="error">
            <Alert severity='error'>{error}</Alert>
        </Snackbar>
        <Snackbar open={loading} autoHideDuration={6000} onClose={ () => setLoading(false) }  data-testid="loading">
          <CircularProgress />
        </Snackbar>
      </header>
    </div>
  );
}

export default App;
