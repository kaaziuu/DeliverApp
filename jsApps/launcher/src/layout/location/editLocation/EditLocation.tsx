import { CircularProgress } from "@mui/material";
import Snackbar from "components/snackbar/Snackbar";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { GetLocationByHashAction, UpdateLocationAction } from "service/location/LocationService";
import UpdateLocationForm, { GetDefaultCreateLocationForm } from "service/location/models/UpdateLocationForm";
import LocationForm from "../_shared/LocationForm";

export type urlParam = {
    locationHash: string;
};

const baseClass = "update-location";

const EditLocation = () => {
    const param = useParams<keyof urlParam>() as urlParam;
    const [form, setForm] = useState<UpdateLocationForm>(GetDefaultCreateLocationForm(param.locationHash));
    const [error, setError] = useState<string | null>(null);
    const [showSnackbar, setShowSnackbar] = useState(false);
    const { isSuccess, isLoading, data } = GetLocationByHashAction(param.locationHash);
    const updateLocation = UpdateLocationAction();

    const submitForm = async () => {
        const response = await updateLocation.mutateAsync(form);
        if (!response.isSuccess) {
            setError(response.error);
        } else {
            setError(null);
        }
        setShowSnackbar(true);
    };

    useEffect(() => {
        if (isSuccess) {
            setForm((prev) => ({ ...prev, ...data! }));
        }
    }, [isLoading, isSuccess, data]);

    if (isLoading) {
        return <CircularProgress />;
    }

    if (!isLoading && !isSuccess) {
        return <p>oops something went wrong. please reload the page and try again</p>;
    }

    return (
        <>
            <LocationForm
                title="Update location"
                submitButtonText="Update"
                baseClass={baseClass}
                error={error}
                form={form}
                setForm={setForm}
                submitForm={submitForm}
            />
            <Snackbar
                message={error ? error : "The location has been update"}
                variant={error ? "error" : "success"}
                isOpen={showSnackbar}
                setIsOpen={setShowSnackbar}
            />
        </>
    );
};

export default EditLocation;
