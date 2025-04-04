import './App.css';
import { useState, useEffect } from 'react';
import axios from 'axios';

function App() {
  const [urls, SetUrls] = useState([]);
  const [user, SetUser] = useState({ id: "", isAuthenticated: false, isAdmin: false });
  const [newUrl, SetNewUrl] = useState("");
  const [errorMessage, SetErrorMessage] = useState("");
  const [successMessage, SetSuccessMessage] = useState("");

  useEffect(() => {
    getUserData();
    getUrls();
  }, []);

  const getUserData = async () => {
    await axios.get("api/auth", { withCredentials: true })
      .then(res => SetUser(res.data))
      .catch(err => SetErrorMessage(err));
    console.log(user);
  }

  const getUrls = async () => {
    await axios.get("api/allurls")
      .then(res => SetUrls(res.data))
      .catch(err => SetErrorMessage(err.response?.data?.error));
    console.log(urls);
  }

  const copyToClipBoard = (text) => {
    navigator.clipboard.writeText(text);
    SetSuccessMessage("Copied!")
  }

  const deteleUrl = async (id) => {
    await axios.post("api/delete", JSON.stringify(id), {
      headers: {
        "Content-Type": "application/json"
      }
    }).catch(err => SetErrorMessage(err.response?.data?.error));
    if (errorMessage === "") {
      SetSuccessMessage("deleted successfully");
    }
    getUrls();
  }

  const createUrl = async (url) => {
    const newUrl = {
      id: "",
      urlOriginal: url,
      createdBy: user.id
    }
    await axios.post("api/create", newUrl, {
      headers: {
        "Content-Type": "application/json"
      }
    }).catch(err => SetErrorMessage(err.response?.data?.error));
    if (errorMessage === "") {
      SetSuccessMessage("created successfully");
    }
    getUrls();
  }

  return (
    <div className="App">
      {errorMessage != "" &&
        <div className='alert alert-danger alert-dismissible fade show' role="alert">
          {errorMessage}
          <button type="button" className='btn-close' data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
      }

      {successMessage != "" &&
        <div className='alert alert-success alert-dismissible fade show' role="alert">
          {successMessage}
          <button type="button" className='btn-close' data-bs-dismiss="alert" aria-label="Close"></button>
        </div>}

      {user != null && user.isAuthenticated &&
        <>
          <input
            type='text'
            className='form-control'
            placeholder='Url'
            onChange={(e) => SetNewUrl(e.target.value)}
          />
          <button onClick={() => createUrl(newUrl)} class="btn btn-primary">Add new</button>
        </>
      }

      <h1>All Shortened Urls</h1>

      {urls.length === 0 ? (<h2>No urls yet</h2>) : (
        <table className='table table-hover'>
          <thead>
            <th>Original Url</th>
            <th>Shortened Url</th>
            <th>Actions</th>
          </thead>
          <tbody>

            {urls.map((item) => (
              <tr>
                <td>
                  {item.urlOriginal}
                </td>
                <td>
                  {item.urlShort}
                </td>
                <td>
                  <div class="btn-group" role="group">
                    <a href={`/short/${item.hash}`} className='btn btn-primary' target='_blank' rel='noopener noreferrer'>Go there</a>
                    <button className='btn btn-success' onClick={() => copyToClipBoard(item.urlShort)}>Copy</button>
                    {user.isAuthenticated && <a href={`/url/info/${item.id}`} className='btn btn-info'>Info</a>}
                    {(user.isAdmin || user.id === item.userId) && <button onClick={() => deteleUrl(item.id)} className='btn btn-danger'>Delete</button>}
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>

  );
}

export default App;
